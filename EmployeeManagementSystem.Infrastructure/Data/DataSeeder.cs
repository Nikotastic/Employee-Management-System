using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        try
        {
            // Apply pending migrations
            Console.WriteLine("🔄 Applying database migrations...");
            await context.Database.MigrateAsync();
            Console.WriteLine("✅ Migrations applied successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Warning applying migrations: {ex.Message}");
            // Continue anyway to seed admin user
        }

        // Seed Admin User (PRIORIDAD MÁXIMA)
        try
        {
            Console.WriteLine("🔍 Checking for existing users...");
            if (!await userManager.Users.AnyAsync())
            {
                Console.WriteLine("👤 Creating administrator user...");
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@talentoplus.com",
                    Email = "admin@talentoplus.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                
                if (result.Succeeded)
                {
                    Console.WriteLine("✅ ========================================");
                    Console.WriteLine("✅ Administrator user created successfully!");
                    Console.WriteLine("✅ Email: admin@talentoplus.com");
                    Console.WriteLine("✅ Password: Admin123!");
                    Console.WriteLine("✅ ========================================");
                }
                else
                {
                    Console.WriteLine("❌ Error creating admin user:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"   - {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine("ℹ️ Admin user already exists");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error creating admin user: {ex.Message}");
            Console.WriteLine($"   Stack: {ex.StackTrace}");
        }

        // Seed Departments
        try
        {
            if (!await context.Departments.AnyAsync())
            {
                Console.WriteLine("📁 Creating departments...");
                var departments = new List<Department>
                {
                    new Department("Recursos Humanos"),
                    new Department("Tecnología"),
                    new Department("Finanzas"),
                    new Department("Marketing"),
                    new Department("Ventas"),
                    new Department("Operaciones"),
                    new Department("Administración")
                };

                await context.Departments.AddRangeAsync(departments);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Departments created");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Warning seeding departments: {ex.Message}");
        }

        // Seed Job Positions
        try
        {
            if (!await context.JobPositions.AnyAsync())
            {
                Console.WriteLine("💼 Creating job positions...");
                var jobPositions = new List<JobPosition>
                {
                    new JobPosition("Director"),
                    new JobPosition("Gerente"),
                    new JobPosition("Coordinador"),
                    new JobPosition("Analista"),
                    new JobPosition("Asistente"),
                    new JobPosition("Auxiliar"),
                    new JobPosition("Desarrollador"),
                    new JobPosition("Diseñador"),
                    new JobPosition("Contador"),
                    new JobPosition("Vendedor")
                };

                await context.JobPositions.AddRangeAsync(jobPositions);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Job positions created");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Warning seeding job positions: {ex.Message}");
        }

        Console.WriteLine("🎉 Database seeding completed!");
    }
}

