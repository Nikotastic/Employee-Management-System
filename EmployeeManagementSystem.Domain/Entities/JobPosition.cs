namespace EmployeeManagementSystem.Domain.Entities;

public class JobPosition
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private JobPosition() { }

    public JobPosition(string name)
    {
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }
}
