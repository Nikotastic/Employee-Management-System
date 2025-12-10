using EmployeeManagementSystem.Domain.Entities;
using FluentAssertions;

namespace EmployeeManagementSystem.Tests.UnitTests;

/// <summary>
/// Pruebas unitarias para las entidades del dominio
/// </summary>
public class DomainEntitiesTests
{
    [Fact]
    public void Department_ShouldBeCreated_WithValidName()
    {
        // Arrange & Act
        var department = new Department("Tecnología");

        // Assert
        department.Should().NotBeNull();
        department.Name.Should().Be("Tecnología");
        department.Id.Should().Be(0); // No asignado hasta guardarse
    }

    [Fact]
    public void JobPosition_ShouldBeCreated_WithValidData()
    {
        // Arrange & Act
        var jobPosition = new JobPosition("Desarrollador Senior");

        // Assert
        jobPosition.Should().NotBeNull();
        jobPosition.Name.Should().Be("Desarrollador Senior");
    }

    [Theory]
    [InlineData("Tecnología")]
    [InlineData("Recursos Humanos")]
    [InlineData("Marketing")]
    [InlineData("Ventas")]
    public void Department_CanBeCreated_WithDifferentNames(string departmentName)
    {
        // Act
        var department = new Department(departmentName);

        // Assert
        department.Name.Should().Be(departmentName);
    }

    [Theory]
    [InlineData("Developer")]
    [InlineData("Manager")]
    [InlineData("Analyst")]
    public void JobPosition_CanBeCreated_WithDifferentNames(string name)
    {
        // Act
        var jobPosition = new JobPosition(name);

        // Assert
        jobPosition.Name.Should().Be(name);
    }
}

