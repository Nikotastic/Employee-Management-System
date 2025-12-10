namespace EmployeeManagementSystem.Application.DTOs;

// DTO for creating a new employee
public class CreateEmployeeDto
{
    public string Document { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int JobPositionId { get; set; }
    public decimal Salary { get; set; }
    public DateTime HiringDate { get; set; }
    public int Status { get; set; }
    public int EducationLevel { get; set; }
    public string ProfessionalProfile { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
}

