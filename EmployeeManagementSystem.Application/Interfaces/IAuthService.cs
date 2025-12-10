using EmployeeManagementSystem.Application.DTOs;

namespace EmployeeManagementSystem.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<bool> RegisterEmployeeAsync(string email, string document);
}

