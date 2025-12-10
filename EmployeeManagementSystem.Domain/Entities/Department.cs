namespace EmployeeManagementSystem.Domain.Entities;

public class Department
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Department() { }

    public Department(string name)
    {
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }
}
