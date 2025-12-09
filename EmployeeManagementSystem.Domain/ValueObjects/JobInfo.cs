using EmployeeManagementSystem.Domain.Enums;

namespace EmployeeManagementSystem.Domain.ValueObjects;

public class JobInfo
{
    public int JobPositionId { get; }
    public decimal Salary { get; }
    public DateTime HiringDate { get; }
    public EmployeeStatus Status { get; }

    public JobInfo(int jobPositionId, decimal salary, DateTime hiringDate, EmployeeStatus status)
    {
        JobPositionId = jobPositionId;
        Salary = salary;
        HiringDate = hiringDate;
        Status = status;
    }
}
