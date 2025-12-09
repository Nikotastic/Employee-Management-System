using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Domain.Services;

public class EmployeeDomainService
{
    public void ValidateNewEmployee(Employee employee)
    {
        if (employee == null)
            throw new ArgumentException("Employee cannot be null.");

        if (employee.JobInfo.Salary < 0)
            throw new ArgumentException("Salary cannot be negative.");
    }
}