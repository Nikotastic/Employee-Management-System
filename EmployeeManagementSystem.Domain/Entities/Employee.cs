using EmployeeManagementSystem.Domain.ValueObjects;

namespace EmployeeManagementSystem.Domain.Entities;

// Employee entity representing an employee in the system
public class Employee
{
    public int Id { get; private set; }
    public string Document { get; private set; } = string.Empty;
    public FullName FullName { get; private set; } = null!;
    public DateTime BirthDate { get; private set; }
    public string Address { get; private set; } = string.Empty;
    public ContactInfo ContactInfo { get; private set; } = null!;
    public JobInfo JobInfo { get; private set; } = null!;
    public EducationInfo EducationInfo { get; private set; } = null!;
    public string ProfessionalProfile { get; private set; } = string.Empty;

    public int DepartmentId { get; private set; }
    public Department Department { get; private set; } = null!;


    private Employee() { }

    public Employee(
        string document,
        FullName fullName,
        DateTime birthDate,
        string address,
        ContactInfo contactInfo,
        JobInfo jobInfo,
        EducationInfo educationInfo,
        string professionalProfile,
        int departmentId)
    {
        Document = document;
        FullName = fullName;
        BirthDate = birthDate;
        Address = address;
        ContactInfo = contactInfo;
        JobInfo = jobInfo;
        EducationInfo = educationInfo;
        ProfessionalProfile = professionalProfile;
        DepartmentId = departmentId;
    }
}
