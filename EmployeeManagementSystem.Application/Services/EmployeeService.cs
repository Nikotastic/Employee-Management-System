using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using EmployeeManagementSystem.Domain.Interfaces;
using EmployeeManagementSystem.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace EmployeeManagementSystem.Application.Services;

// Service implementation for managing employees
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IJobPositionRepository _jobPositionRepository;
    private readonly IExcelService _excelService;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IJobPositionRepository jobPositionRepository,
        IExcelService excelService,
        ILogger<EmployeeService> logger)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _jobPositionRepository = jobPositionRepository;
        _excelService = excelService;
        _logger = logger;
    }

    // Get all employees with job position names
    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        var employeeDtos = new List<EmployeeDto>();
        
        foreach (var employee in employees)
        {
            var dto = MapToDto(employee);
            var jobPosition = await _jobPositionRepository.GetByIdAsync(employee.JobInfo.JobPositionId);
            dto.JobPositionName = jobPosition?.Name ?? "";
            employeeDtos.Add(dto);
        }
        
        return employeeDtos;
    }

    // Get paginated employees with job position names
    public async Task<PaginatedResult<EmployeeDto>> GetEmployeesPagedAsync(int pageNumber, int pageSize)
    {
        var (employees, totalCount) = await _employeeRepository.GetPagedAsync(pageNumber, pageSize);
        var employeeDtos = new List<EmployeeDto>();
        
        foreach (var employee in employees)
        {
            var dto = MapToDto(employee);
            var jobPosition = await _jobPositionRepository.GetByIdAsync(employee.JobInfo.JobPositionId);
            dto.JobPositionName = jobPosition?.Name ?? "";
            employeeDtos.Add(dto);
        }
        
        return new PaginatedResult<EmployeeDto>
        {
            Items = employeeDtos,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    // Get employee by ID with job position name
    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return null;
        
        var dto = MapToDto(employee);
        var jobPosition = await _jobPositionRepository.GetByIdAsync(employee.JobInfo.JobPositionId);
        dto.JobPositionName = jobPosition?.Name ?? "";
        
        return dto;
    }

    // Get employee by document with job position name
    public async Task<EmployeeDto?> GetEmployeeByDocumentAsync(string document)
    {
        var employee = await _employeeRepository.GetByDocumentIdAsync(document);
        if (employee == null) return null;
        
        var dto = MapToDto(employee);
        var jobPosition = await _jobPositionRepository.GetByIdAsync(employee.JobInfo.JobPositionId);
        dto.JobPositionName = jobPosition?.Name ?? "";
        
        return dto;
    }

    // Create a new employee
    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        // Validate that there is no employee with the same document
        var existing = await _employeeRepository.GetByDocumentIdAsync(dto.Document);
        if (existing != null)
        {
            throw new InvalidOperationException($"An employee already exists with the document {dto.Document}");
        }

        // Create the Employee entity
        var employee = new Employee(
            document: dto.Document,
            fullName: new FullName(dto.FirstName, dto.LastName),
            birthDate: dto.BirthDate,
            address: dto.Address,
            contactInfo: new ContactInfo(dto.Email, dto.Phone),
            jobInfo: new JobInfo(dto.JobPositionId, dto.Salary, dto.HiringDate, (EmployeeStatus)dto.Status),
            educationInfo: new EducationInfo((EducationLevel)dto.EducationLevel),
            professionalProfile: dto.ProfessionalProfile,
            departmentId: dto.DepartmentId
        );

        await _employeeRepository.AddAsync(employee);
        
        return MapToDto(employee);
    }
    // Update an existing employee

    public async Task UpdateEmployeeAsync(int id, CreateEmployeeDto dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee ID not found {id}");
        }

        // Create a new entity with the updated data
        var updatedEmployee = new Employee(
            document: dto.Document,
            fullName: new FullName(dto.FirstName, dto.LastName),
            birthDate: dto.BirthDate,
            address: dto.Address,
            contactInfo: new ContactInfo(dto.Email, dto.Phone),
            jobInfo: new JobInfo(dto.JobPositionId, dto.Salary, dto.HiringDate, (EmployeeStatus)dto.Status),
            educationInfo: new EducationInfo((EducationLevel)dto.EducationLevel),
            professionalProfile: dto.ProfessionalProfile,
            departmentId: dto.DepartmentId
        );

        // CRITICAL: Set the ID of the existing employee to ensure EF Core updates the correct record
        // Using reflection to set the private Id property
        var idProperty = typeof(Employee).GetProperty("Id");
        if (idProperty != null)
        {
            idProperty.SetValue(updatedEmployee, id);
        }

        await _employeeRepository.UpdateAsync(updatedEmployee);
    }

    // Delete an employee by ID
    public async Task DeleteEmployeeAsync(int id)
    {
        await _employeeRepository.DeleteAsync(id);
    }

    public async Task<int> ImportEmployeesFromExcelAsync(Stream fileStream)
    {
        try
        {
            var employeeDtos = await _excelService.ImportEmployeesFromExcelAsync(fileStream);
            var importedCount = 0;

            foreach (var dto in employeeDtos)
            {
                try
                {
                    // 1. Search or create Job Position and Department FIRST (needed for both create and update)
                    if (!string.IsNullOrEmpty(dto.JobPositionName))
                    {
                        var jobPosition = await _jobPositionRepository.GetByNameAsync(dto.JobPositionName);
                        if (jobPosition == null)
                        {
                            jobPosition = new JobPosition(dto.JobPositionName);
                            await _jobPositionRepository.AddAsync(jobPosition);
                            _logger.LogInformation("JobPosition created: {Name}", dto.JobPositionName);
                        }
                        dto.JobPositionId = jobPosition.Id;
                    }

                    if (!string.IsNullOrEmpty(dto.DepartmentName))
                    {
                        var department = await _departmentRepository.GetByNameAsync(dto.DepartmentName);
                        if (department == null)
                        {
                            department = new Department(dto.DepartmentName);
                            await _departmentRepository.AddAsync(department);
                            _logger.LogInformation("Department created: {Name}", dto.DepartmentName);
                        }
                        dto.DepartmentId = department.Id;
                    }

                    // 2. Validate IDs
                    if (dto.JobPositionId <= 0 || dto.DepartmentId <= 0)
                    {
                         _logger.LogWarning("Employee {Document} skipped: Invalid Job or Department", dto.Document);
                         continue;
                    }

                    // 3. Check if employee exists
                    var existing = await _employeeRepository.GetByDocumentIdAsync(dto.Document);
                    
                    if (existing != null)
                    {
                        // UPDATE EXISTING
                        // We map the incoming DTO to the Update Logic
                        // Since UpdateEmployeeAsync takes CreateEmployeeDto, we can reuse it or manually update properties
                        
                        // Manually updating the entity to ensure persistence
                        // Note: Depending on your repository pattern, you might need to Attach/Entry modify. 
                        // Assuming UpdateAsync handles a detached or attached entity correctly.
                        
                        // We need to use the ID of the EXISTING employee to update it
                        await UpdateEmployeeAsync(existing.Id, dto);
                        _logger.LogInformation("Employee updated: {Document}", dto.Document);
                        // We count updates as imports/process items
                        importedCount++;
                    }
                    else
                    {
                        // CREATE NEW
                        await CreateEmployeeAsync(dto);
                        importedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when importing employee with document {Document}", dto.Document);
                }
            }

            return importedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing Excel file");
            throw;
        }
    }

    private EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            Document = employee.Document,
            FirstName = employee.FullName.FirstName,
            LastName = employee.FullName.LastName,
            BirthDate = employee.BirthDate,
            Address = employee.Address,
            Email = employee.ContactInfo.Email,
            Phone = employee.ContactInfo.Phone,
            JobPositionId = employee.JobInfo.JobPositionId,
            JobPositionName = "",
            Salary = employee.JobInfo.Salary,
            HiringDate = employee.JobInfo.HiringDate,
            Status = employee.JobInfo.Status.ToString(),
            EducationLevel = employee.EducationInfo.Level.ToString(),
            ProfessionalProfile = employee.ProfessionalProfile,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name ?? ""
        };
    }
}

