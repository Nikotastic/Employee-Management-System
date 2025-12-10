using EmployeeManagementSystem.Application.DTOs;

namespace EmployeeManagementSystem.Application.Interfaces;

// Service for handling Excel file operations
public interface IExcelService
{
    Task<List<ImportEmployeeDto>> ImportEmployeesFromExcelAsync(Stream fileStream);
}

