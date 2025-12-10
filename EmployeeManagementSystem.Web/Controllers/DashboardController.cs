using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Web.Models;

namespace EmployeeManagementSystem.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IAIService _aiService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IEmployeeService employeeService,
        IAIService aiService,
        ILogger<DashboardController> logger)
    {
        _employeeService = employeeService;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var employeesList = employees?.ToList() ?? new List<EmployeeManagementSystem.Application.DTOs.EmployeeDto>();

            var model = new DashboardViewModel
            {
                TotalEmployees = employeesList.Count,
                ActiveEmployees = employeesList.Count(e => e.Status == "Active"),
                OnVacationEmployees = employeesList.Count(e => e.Status == "Vacation"),
                InactiveEmployees = employeesList.Count(e => e.Status == "Inactive"),
                RecentEmployees = employeesList.OrderByDescending(e => e.HiringDate).Take(5).ToList()
            };

            _logger.LogInformation($"✅ Dashboard loaded successfully. Total employees: {employeesList.Count}");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Error loading dashboard, returning empty model");
            // Retornar un modelo vacío pero válido
            return View(new DashboardViewModel
            {
                TotalEmployees = 0,
                ActiveEmployees = 0,
                OnVacationEmployees = 0,
                InactiveEmployees = 0,
                RecentEmployees = new List<EmployeeManagementSystem.Application.DTOs.EmployeeDto>()
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AskAI([FromBody] AIQueryRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest(new { error = "La consulta no puede estar vacía" });
            }

            // Obtener todos los empleados para el contexto
            var employees = await _employeeService.GetAllEmployeesAsync();
            var employeesList = employees.ToList();

            // Construir contexto para la IA
            var context = $@"
Tienes acceso a una base de datos de empleados con la siguiente información:
- Total de empleados: {employeesList.Count}
- Empleados activos: {employeesList.Count(e => e.Status == "Active")}
- Empleados en vacaciones: {employeesList.Count(e => e.Status == "Vacation")}
- Empleados inactivos: {employeesList.Count(e => e.Status == "Inactive")}

Departamentos y cantidad de empleados:
{string.Join("\n", employeesList.GroupBy(e => e.DepartmentName).Select(g => $"- {g.Key}: {g.Count()} empleados"))}

Cargos y cantidad:
{string.Join("\n", employeesList.GroupBy(e => e.JobPositionName).Select(g => $"- {g.Key}: {g.Count()} empleados"))}

Niveles educativos:
{string.Join("\n", employeesList.GroupBy(e => e.EducationLevel).Select(g => $"- {g.Key}: {g.Count()} empleados"))}

Por favor responde la siguiente pregunta basándote en estos datos reales: {request.Query}
";

            var response = await _aiService.ProcessQueryAsync(context);

            return Ok(new { response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar consulta de IA");
            return StatusCode(500, new { error = "Error al procesar la consulta", details = ex.Message });
        }
    }
}

public class AIQueryRequest
{
    public string Query { get; set; } = string.Empty;
}

