namespace EmployeeManagementSystem.Domain.ValueObjects;

public class FullName
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    private FullName()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.");

        FirstName = firstName;
        LastName = lastName;
    }
}