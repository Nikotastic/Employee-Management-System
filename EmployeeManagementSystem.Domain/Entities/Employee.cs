using EmployeeManagementSystem.Domain.ValueObjects;

namespace EmployeeManagementSystem.Domain.Entities;

public class Employee
{
    public int Id { get; private set; }
    public string Document { get; private set; }
    public FullName FullName { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string Address { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public JobInfo JobInfo { get; private set; }
    public EducationInfo EducationInfo { get; private set; }
    public string ProfessionalProfile { get; private set; }

    public int DepartmentId { get; private set; }
    public Department Department { get; private set; }

    public Employee() { }

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
