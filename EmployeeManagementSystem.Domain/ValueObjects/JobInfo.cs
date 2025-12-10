using EmployeeManagementSystem.Domain.Enums;

namespace EmployeeManagementSystem.Domain.ValueObjects;

public class JobInfo
{
    public int JobPositionId { get; private set; }
    public decimal Salary { get; private set; }
    public DateTime HiringDate { get; private set; }
    public EmployeeStatus Status { get; private set; }

    private JobInfo()
    {
        JobPositionId = 0;
        Salary = 0;
        HiringDate = DateTime.MinValue;
        Status = EmployeeStatus.Active;
    }

    public JobInfo(int jobPositionId, decimal salary, DateTime hiringDate, EmployeeStatus status)
    {
        JobPositionId = jobPositionId;
        Salary = salary;
        HiringDate = hiringDate;
        Status = status;
    }
}
