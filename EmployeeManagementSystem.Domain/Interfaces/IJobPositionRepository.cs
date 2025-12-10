using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Domain.Interfaces;

// Repository interface for JobPosition entity
public interface IJobPositionRepository
{
    Task<JobPosition?> GetByIdAsync(int id);
    Task<IEnumerable<JobPosition>> GetAllAsync();
    Task<JobPosition?> GetByNameAsync(string name);
    Task AddAsync(JobPosition jobPosition);
}

