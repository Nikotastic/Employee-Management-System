using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(
        IDepartmentRepository departmentRepository,
        ILogger<DepartmentsController> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los departamentos - Endpoint público
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync();
            var dtos = departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name
            });

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener departamentos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}

