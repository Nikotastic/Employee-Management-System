using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Interfaces;
using System.Linq;

namespace EmployeeManagementSystem.Web.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IJobPositionRepository _jobPositionRepository;
    private readonly IPdfService _pdfService;
    private readonly IExcelService _excelService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        IEmployeeService employeeService,
        IDepartmentRepository departmentRepository,
        IJobPositionRepository jobPositionRepository,
        IPdfService pdfService,
        IExcelService excelService,
        ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _departmentRepository = departmentRepository;
        _jobPositionRepository = jobPositionRepository;
        _pdfService = pdfService;
        _excelService = excelService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        try
        {
            // Ensure page is at least 1
            if (page < 1) page = 1;
            
            // Limit page size between 5 and 100
            if (pageSize < 5) pageSize = 5;
            if (pageSize > 100) pageSize = 100;
            
            var paginatedEmployees = await _employeeService.GetEmployeesPagedAsync(page, pageSize);
            return View(paginatedEmployees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar empleados");
            TempData["Error"] = "Error al cargar los empleados";
            return View(new PaginatedResult<EmployeeDto> { Items = new List<EmployeeDto>(), CurrentPage = 1, PageSize = pageSize, TotalItems = 0 });
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar detalles del empleado {Id}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEmployeeDto dto)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _employeeService.CreateEmployeeAsync(dto);
                TempData["Success"] = "Empleado creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear empleado");
                ModelState.AddModelError("", $"Error al crear el empleado: {ex.Message}");
            }
        }

        await PopulateDropdowns();
        return View(dto);
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var dto = new CreateEmployeeDto
            {
                Document = employee.Document,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                BirthDate = employee.BirthDate,
                Address = employee.Address,
                Email = employee.Email,
                Phone = employee.Phone,
                JobPositionId = employee.JobPositionId,
                Salary = employee.Salary,
                HiringDate = employee.HiringDate,
                Status = (int)Enum.Parse<Domain.Enums.EmployeeStatus>(employee.Status),
                EducationLevel = (int)Enum.Parse<Domain.Enums.EducationLevel>(employee.EducationLevel),
                ProfessionalProfile = employee.ProfessionalProfile,
                DepartmentId = employee.DepartmentId
            };

            await PopulateDropdowns();
            ViewBag.EmployeeId = id;
            return View(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar empleado para editar {Id}", id);
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateEmployeeDto dto)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _employeeService.UpdateEmployeeAsync(id, dto);
                TempData["Success"] = "Empleado actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar empleado {Id}", id);
                ModelState.AddModelError("", $"Error al actualizar el empleado: {ex.Message}");
            }
        }

        await PopulateDropdowns();
        ViewBag.EmployeeId = id;
        return View(dto);
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar empleado para eliminar {Id}", id);
            return NotFound();
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _employeeService.DeleteEmployeeAsync(id);
            TempData["Success"] = "Empleado eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar empleado {Id}", id);
            TempData["Error"] = $"Error al eliminar el empleado: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> DownloadCV(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var pdfBytes = await _pdfService.GenerateEmployeeCvPdfAsync(employee);
            return File(pdfBytes, "application/pdf", $"CV_{employee.FirstName}_{employee.LastName}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar PDF para empleado {Id}", id);
            TempData["Error"] = "Error al generar el PDF";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult ImportExcel()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "Por favor seleccione un archivo");
            return View();
        }

        if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
        {
            ModelState.AddModelError("", "El archivo debe ser un Excel (.xlsx o .xls)");
            return View();
        }

        try
        {
            using var stream = file.OpenReadStream();
            var importedCount = await _employeeService.ImportEmployeesFromExcelAsync(stream);
            
            TempData["Success"] = $"Se importaron {importedCount} empleados exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar Excel");
            ModelState.AddModelError("", $"Error al importar el archivo: {ex.Message}");
            return View();
        }
    }

    private async Task PopulateDropdowns()
    {
        var departments = await _departmentRepository.GetAllAsync();
        var jobPositions = await _jobPositionRepository.GetAllAsync();

        ViewBag.Departments = new SelectList(departments, "Id", "Name");
        ViewBag.JobPositions = new SelectList(jobPositions, "Id", "Name");
        // Ensure values are Cast to int so validation passes
        ViewBag.EmployeeStatuses = new SelectList(Enum.GetValues(typeof(Domain.Enums.EmployeeStatus))
            .Cast<Domain.Enums.EmployeeStatus>()
            .Select(e => new { Id = (int)e, Name = e.ToString() }), "Id", "Name");
            
        ViewBag.EducationLevels = new SelectList(Enum.GetValues(typeof(Domain.Enums.EducationLevel))
            .Cast<Domain.Enums.EducationLevel>()
            .Select(e => new { Id = (int)e, Name = e.ToString() }), "Id", "Name");
    }
}

