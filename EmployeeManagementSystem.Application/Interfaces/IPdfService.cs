using EmployeeManagementSystem.Application.DTOs;

namespace EmployeeManagementSystem.Application.Interfaces;

// Service for generating PDF documents
public interface IPdfService
{
    Task<byte[]> GenerateEmployeeCvPdfAsync(EmployeeDto employee);
}

