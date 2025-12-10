using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Interfaces;
using EmployeeManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _dbContext;
    public EmployeeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    

    public async Task AddAsync(Employee employee)
    {
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _dbContext.Employees.FindAsync(id);
        if (e != null)
        {
            _dbContext.Employees.Remove(e);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _dbContext.Employees
            .Include(e => e.Department)
            .ToListAsync();
    }

    public async Task<Employee?> GetByDocumentIdAsync(string documentId)
    {
        return await _dbContext.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(x => x.Document == documentId);
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _dbContext.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(Employee employee)
    {
        _dbContext.Employees.Update(employee);
        await _dbContext.SaveChangesAsync();
    }
}