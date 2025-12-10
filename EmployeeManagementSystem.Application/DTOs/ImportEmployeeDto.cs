namespace EmployeeManagementSystem.Application.DTOs;

// DTO for importing employee data with job position and department names
public class ImportEmployeeDto : CreateEmployeeDto
{
    public string JobPositionName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
}

