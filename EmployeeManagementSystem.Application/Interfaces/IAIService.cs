namespace EmployeeManagementSystem.Application.Interfaces;

// Service for AI-related functionalities
public interface IAIService
{
    Task<string> ProcessQueryAsync(string query);
}

