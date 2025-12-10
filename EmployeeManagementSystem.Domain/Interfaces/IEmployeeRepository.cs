using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Domain.Interfaces;

// Repository interface for Employee entity
public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByDocumentIdAsync(string documentId);
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int id);
}