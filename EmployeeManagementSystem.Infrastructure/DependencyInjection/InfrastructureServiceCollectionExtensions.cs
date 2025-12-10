using System.Text;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Interfaces;
using EmployeeManagementSystem.Infrastructure.Data;
using EmployeeManagementSystem.Infrastructure.Identity;
using EmployeeManagementSystem.Infrastructure.Repositories;
using EmployeeManagementSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagementSystem.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // JWT Authentication
        var jwtKey = configuration["JWT:Key"];
        var jwtIssuer = configuration["JWT:Issuer"];
        var jwtAudience = configuration["JWT:Audience"];

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT Key no configurada")))
            };
        });

        // Repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IJobPositionRepository, JobPositionRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddScoped<IAIService, GeminiAIService>();
        
        // HttpClient para servicios externos
        services.AddHttpClient();

        return services;
    }
}