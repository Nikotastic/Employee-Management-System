using EmployeeManagementSystem.Application.DTOs;

namespace EmployeeManagementSystem.Application.Interfaces;

// Service interface for managing employees
public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    Task<PaginatedResult<EmployeeDto>> GetEmployeesPagedAsync(int pageNumber, int pageSize);
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
    Task<EmployeeDto?> GetEmployeeByDocumentAsync(string document);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);
    Task UpdateEmployeeAsync(int id, CreateEmployeeDto dto);
    Task DeleteEmployeeAsync(int id);
    Task<int> ImportEmployeesFromExcelAsync(Stream fileStream);
}

