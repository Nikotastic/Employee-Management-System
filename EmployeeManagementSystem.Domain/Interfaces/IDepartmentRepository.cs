using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Domain.Interfaces;

// Repository interface for Department entity
public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<Department?> GetByNameAsync(string name);
    Task AddAsync(Department department);
}