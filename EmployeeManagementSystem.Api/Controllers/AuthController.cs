using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login para empleados - Genera un token JWT
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            
            if (result == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}

