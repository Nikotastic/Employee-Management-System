using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagementSystem.Infrastructure.Services;

// Service implementation for authentication
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    // User login method
    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", loginDto.Email);
                return null;
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                _logger.LogWarning("Incorrect password for: {Email}", loginDto.Email);
                return null;
            }

            var token = GenerateJwtToken(user);
            var expiryMinutes = int.Parse(_configuration["JWT:ExpiryMinutes"] ?? "15");

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? string.Empty,
                Expiration = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for {Email}", loginDto.Email);
            return null;
        }
    }

    // Employee registration method
    public async Task<bool> RegisterEmployeeAsync(string email, string document)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                _logger.LogWarning("User already exists: {Email}", email);
                return false;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmployeeDocument = document,
                EmailConfirmed = true
            };

            // Generate a temporary password (the document)
            var result = await _userManager.CreateAsync(user, document);

            if (result.Succeeded)
            {
                _logger.LogInformation("Successfully registered user: {Email}", email);
                return true;
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError("Error registering user: {Error}", error.Description);
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering employee: {Email}", email);
            return false;
        }
    }

    // JWT token generation method
    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = _configuration["JWT:Key"];
        var issuer = _configuration["JWT:Issuer"];
        var audience = _configuration["JWT:Audience"];
        var expiryMinutes = int.Parse(_configuration["JWT:ExpiryMinutes"] ?? "15");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? throw new InvalidOperationException("JWT Key no configurada")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("document", user.EmployeeDocument ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

