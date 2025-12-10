namespace EmployeeManagementSystem.Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string Document { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int JobPositionId { get; set; }
    public string JobPositionName { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime HiringDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public string ProfessionalProfile { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}

