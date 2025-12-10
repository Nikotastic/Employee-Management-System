using Microsoft.AspNetCore.Identity;

namespace EmployeeManagementSystem.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? EmployeeDocument { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

