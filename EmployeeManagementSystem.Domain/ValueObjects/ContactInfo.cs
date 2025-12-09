namespace EmployeeManagementSystem.Domain.ValueObjects;

public class ContactInfo
{
    public string Email { get; }
    public string Phone { get; }

    public ContactInfo(string email, string phone)
    {
        Email = email;
        Phone = phone;
    }
}
