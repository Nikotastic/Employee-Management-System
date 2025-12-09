namespace EmployeeManagementSystem.Domain.Entities;

public class Department
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public Department(string name)
    {
        Name = name;
    }
}
