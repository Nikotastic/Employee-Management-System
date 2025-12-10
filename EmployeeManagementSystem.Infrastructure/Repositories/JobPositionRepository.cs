using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Interfaces;
using EmployeeManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Infrastructure.Repositories;

public class JobPositionRepository : IJobPositionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public JobPositionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<JobPosition?> GetByIdAsync(int id)
    {
        return await _dbContext.JobPositions.FindAsync(id);
    }

    public async Task<IEnumerable<JobPosition>> GetAllAsync()
    {
        return await _dbContext.JobPositions.ToListAsync();
    }

    public async Task<JobPosition?> GetByNameAsync(string name)
    {
        return await _dbContext.JobPositions
            .FirstOrDefaultAsync(jp => jp.Name == name);
    }

    public async Task AddAsync(JobPosition jobPosition)
    {
        _dbContext.JobPositions.Add(jobPosition);
        await _dbContext.SaveChangesAsync();
    }
}

