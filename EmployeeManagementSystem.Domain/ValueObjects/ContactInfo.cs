namespace EmployeeManagementSystem.Domain.ValueObjects;

public class ContactInfo
{
    public string Email { get; private set; }
    public string Phone { get; private set; }

    private ContactInfo() 
    {
        Email = string.Empty;
        Phone = string.Empty;
    }

    public ContactInfo(string email, string phone)
    {
        Email = email;
        Phone = phone;
    }
}
