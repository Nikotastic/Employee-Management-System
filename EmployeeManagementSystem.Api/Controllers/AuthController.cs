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
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IEmailService emailService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _emailService = emailService;
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

    /// <summary>
    /// Autoregistro de empleados - Crea usuario y envía correo
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Document))
            {
                return BadRequest(new { message = "Email y Documento son requeridos" });
            }

            var result = await _authService.RegisterEmployeeAsync(registerDto.Email, registerDto.Document);
            
            if (!result)
            {
                return BadRequest(new { message = "Error al registrar. Verifique si el usuario ya existe." });
            }

            // Enviar correo de bienvenida
            try
            {
                await _emailService.SendWelcomeEmailAsync(registerDto.Email, $"{registerDto.FirstName} {registerDto.LastName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de bienvenida a {Email}", registerDto.Email);
                // No fallamos el registro si falla el correo, pero lo logueamos. 
                // O deberíamos advertir al usuario? "Registro exitoso pero no se pudo enviar el correo".
            }

            return Ok(new { message = "Registro exitoso. Se ha enviado un correo de bienvenida." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en registro");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}

