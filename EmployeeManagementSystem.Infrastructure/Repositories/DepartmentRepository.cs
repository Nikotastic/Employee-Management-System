using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Interfaces;
using EmployeeManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Infrastructure.Repositories;

public class DepartmentRepository: IDepartmentRepository
{
    private readonly ApplicationDbContext _context;
    public DepartmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        return await _context.Departments.FindAsync(id);
    }

    public async Task<Department?> GetByNameAsync(string name)
    {
        return await _context.Departments.FirstOrDefaultAsync(d => d.Name == name);
    }
}