using System.Security.Claims;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IPdfService _pdfService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        IEmployeeService employeeService,
        IAuthService authService,
        IEmailService emailService,
        IPdfService pdfService,
        ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _authService = authService;
        _emailService = emailService;
        _pdfService = pdfService;
        _logger = logger;
    }
    
    // Employee self-registration - Public Endpoint
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] CreateEmployeeDto dto)
    {
        try
        {
            // Create the employee
            var employee = await _employeeService.CreateEmployeeAsync(dto);

            // Register user in Identity
            var userCreated = await _authService.RegisterEmployeeAsync(dto.Email, dto.Document);

            if (!userCreated)
            {
                return BadRequest(new { message = "Error al crear el usuario" });
            }

            // Send welcome email
            try
            {
                await _emailService.SendWelcomeEmailAsync(dto.Email, $"{dto.FirstName} {dto.LastName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending welcome email");
            }

            return Ok(new
            {
                    message = "Successfully registered employee. A confirmation email has been sent.",
                employee
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering employee");
            return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
        }
    }
    
    // Get authenticated employee information - Protected with JWT
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<EmployeeDto>> GetMyInfo()
    {
        try
        {
            var document = User.FindFirst("document")?.Value;

            if (string.IsNullOrEmpty(document))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var employee = await _employeeService.GetEmployeeByDocumentAsync(document);

            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employee information");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
    
    // Download authenticated employee resume - Protected with JWT
    [HttpGet("me/cv")]
    [Authorize]
    public async Task<ActionResult> DownloadMyCv()
    {
        try
        {
            var document = User.FindFirst("document")?.Value;

            if (string.IsNullOrEmpty(document))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var employee = await _employeeService.GetEmployeeByDocumentAsync(document);

            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            var pdfBytes = await _pdfService.GenerateEmployeeCvPdfAsync(employee);

            return File(pdfBytes, "application/pdf", $"CV_{employee.FirstName}_{employee.LastName}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
    
    // Get all employees - Requires authentication (admin)
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employees");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
    
    // Get employee by ID - Requires authentication (admin)
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employee");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
    
    // Generate PDF of an employee's resume - Requires authentication (admin)
    [HttpGet("{id}/cv")]
    [Authorize]
    public async Task<ActionResult> DownloadEmployeeCv(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            var pdfBytes = await _pdfService.GenerateEmployeeCvPdfAsync(employee);

            return File(pdfBytes, "application/pdf", $"CV_{employee.FirstName}_{employee.LastName}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
    
    // Update Employee - Requires authentication (admin)
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, [FromBody] CreateEmployeeDto dto)
    {
        try
        {
            await _employeeService.UpdateEmployeeAsync(id, dto);
            return Ok(new { message = "Employee successfully updated" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Employee not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
    
    // Delete employee - Requires authentication (admin)
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return Ok(new { message = "Employee successfully removed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
    
    // Import employees from Excel file - Requires authentication (admin)
    [HttpPost("import-excel")]
    [Authorize]
    public async Task<ActionResult> ImportFromExcel(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Invalid file" });
            }

            if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
            {
                return BadRequest(new { message = "The file must be an Excel (.xlsx or .xls)" });
            }

            using var stream = file.OpenReadStream();
            var importedCount = await _employeeService.ImportEmployeesFromExcelAsync(stream);

            return Ok(new
            {
                message = $"{importedCount} employees were successfully imported",
                count = importedCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing Excel");
            return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
        }
    }
}

