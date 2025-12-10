using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        
        // Register FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        // Register Application Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        
        return services;
    }
}

