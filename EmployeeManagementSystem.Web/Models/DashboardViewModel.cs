using EmployeeManagementSystem.Application.DTOs;

namespace EmployeeManagementSystem.Web.Models;

public class DashboardViewModel
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int OnVacationEmployees { get; set; }
    public int InactiveEmployees { get; set; }
    public List<EmployeeDto> RecentEmployees { get; set; } = new();
}
