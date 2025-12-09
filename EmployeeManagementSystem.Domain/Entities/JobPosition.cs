namespace EmployeeManagementSystem.Domain.Entities;

public class JobPosition
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public JobPosition(string name)
    {
        Name = name;
    }
}
