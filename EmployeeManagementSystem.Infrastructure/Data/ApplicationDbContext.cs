using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Infrastructure.Identity;
using EmployeeManagementSystem.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<JobPosition> JobPositions => Set<JobPosition>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Ignore Value Objects (they are not entities)
        modelBuilder.Ignore<EmployeeManagementSystem.Domain.ValueObjects.ContactInfo>();
        modelBuilder.Ignore<EmployeeManagementSystem.Domain.ValueObjects.FullName>();
        modelBuilder.Ignore<EmployeeManagementSystem.Domain.ValueObjects.JobInfo>();
        modelBuilder.Ignore<EmployeeManagementSystem.Domain.ValueObjects.EducationInfo>();
        
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new JobPositionConfiguration());
    }
}