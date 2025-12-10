using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using EmployeeManagementSystem.Domain.ValueObjects;
using FluentAssertions;

namespace EmployeeManagementSystem.Tests.UnitTests;

/// <summary>
/// Pruebas unitarias para la entidad Employee
/// </summary>
public class EmployeeEntityTests
{
    [Fact]
    public void Employee_ShouldBeCreated_WithValidData()
    {
        // Arrange
        var document = "1234567890";
        var fullName = new FullName("Juan", "Pérez");
        var birthDate = new DateTime(1990, 5, 15);
        var address = "Calle 123 #45-67";
        var contactInfo = new ContactInfo("juan.perez@example.com", "3001234567");
        var jobInfo = new JobInfo(1, 3500000, DateTime.Now, EmployeeStatus.Active);
        var educationInfo = new EducationInfo(EducationLevel.Professional);
        var professionalProfile = "Desarrollador de software con 5 años de experiencia";
        var departmentId = 1;

        // Act
        var employee = new Employee(
            document: document,
            fullName: fullName,
            birthDate: birthDate,
            address: address,
            contactInfo: contactInfo,
            jobInfo: jobInfo,
            educationInfo: educationInfo,
            professionalProfile: professionalProfile,
            departmentId: departmentId
        );

        // Assert
        employee.Should().NotBeNull();
        employee.Document.Should().Be(document);
        employee.FullName.Should().Be(fullName);
        employee.BirthDate.Should().Be(birthDate);
        employee.Address.Should().Be(address);
        employee.ContactInfo.Should().Be(contactInfo);
        employee.JobInfo.Should().Be(jobInfo);
        employee.EducationInfo.Should().Be(educationInfo);
        employee.ProfessionalProfile.Should().Be(professionalProfile);
        employee.DepartmentId.Should().Be(departmentId);
    }

    [Fact]
    public void FullName_ShouldReturnCorrectFullName()
    {
        // Arrange & Act
        var fullName = new FullName("María", "González");

        // Assert
        fullName.FirstName.Should().Be("María");
        fullName.LastName.Should().Be("González");
    }

    [Fact]
    public void ContactInfo_ShouldBeCreated_WithValidEmailAndPhone()
    {
        // Arrange & Act
        var contactInfo = new ContactInfo("test@example.com", "3001234567");

        // Assert
        contactInfo.Email.Should().Be("test@example.com");
        contactInfo.Phone.Should().Be("3001234567");
    }

    [Fact]
    public void JobInfo_ShouldBeCreated_WithValidData()
    {
        // Arrange
        var jobPositionId = 1;
        var salary = 4500000m;
        var hiringDate = new DateTime(2023, 1, 15);
        var status = EmployeeStatus.Active;

        // Act
        var jobInfo = new JobInfo(jobPositionId, salary, hiringDate, status);

        // Assert
        jobInfo.JobPositionId.Should().Be(jobPositionId);
        jobInfo.Salary.Should().Be(salary);
        jobInfo.HiringDate.Should().Be(hiringDate);
        jobInfo.Status.Should().Be(status);
    }

    [Fact]
    public void EducationInfo_ShouldBeCreated_WithValidEducationLevel()
    {
        // Arrange & Act
        var educationInfo = new EducationInfo(EducationLevel.Master);

        // Assert
        educationInfo.Level.Should().Be(EducationLevel.Master);
    }

    [Theory]
    [InlineData(EmployeeStatus.Active)]
    [InlineData(EmployeeStatus.Inactive)]
    [InlineData(EmployeeStatus.Vacation)]
    public void Employee_CanHaveDifferentStatuses(EmployeeStatus status)
    {
        // Arrange
        var jobInfo = new JobInfo(1, 3000000, DateTime.Now, status);
        var employee = new Employee(
            document: "123456",
            fullName: new FullName("Test", "User"),
            birthDate: new DateTime(1990, 1, 1),
            address: "Test Address",
            contactInfo: new ContactInfo("test@test.com", "3001234567"),
            jobInfo: jobInfo,
            educationInfo: new EducationInfo(EducationLevel.Professional),
            professionalProfile: "Test Profile",
            departmentId: 1
        );

        // Assert
        employee.JobInfo.Status.Should().Be(status);
    }

    [Theory]
    [InlineData(EducationLevel.HighSchool)]
    [InlineData(EducationLevel.Technical)]
    [InlineData(EducationLevel.Technologist)]
    [InlineData(EducationLevel.Professional)]
    [InlineData(EducationLevel.Specialization)]
    [InlineData(EducationLevel.Master)]
    [InlineData(EducationLevel.Doctorate)]
    public void Employee_CanHaveDifferentEducationLevels(EducationLevel level)
    {
        // Arrange
        var educationInfo = new EducationInfo(level);
        var employee = new Employee(
            document: "123456",
            fullName: new FullName("Test", "User"),
            birthDate: new DateTime(1990, 1, 1),
            address: "Test Address",
            contactInfo: new ContactInfo("test@test.com", "3001234567"),
            jobInfo: new JobInfo(1, 3000000, DateTime.Now, EmployeeStatus.Active),
            educationInfo: educationInfo,
            professionalProfile: "Test Profile",
            departmentId: 1
        );

        // Assert
        employee.EducationInfo.Level.Should().Be(level);
    }
}

