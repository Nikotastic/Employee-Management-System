using EmployeeManagementSystem.Domain.Enums;
using FluentAssertions;

namespace EmployeeManagementSystem.Tests.UnitTests;

/// <summary>
/// Pruebas unitarias para los enums del dominio
/// </summary>
public class EnumsTests
{
    [Fact]
    public void EmployeeStatus_ShouldHaveCorrectValues()
    {
        // Assert
        ((int)EmployeeStatus.Active).Should().Be(1);
        ((int)EmployeeStatus.Inactive).Should().Be(2);
        ((int)EmployeeStatus.Vacation).Should().Be(3);
    }

    [Fact]
    public void EducationLevel_ShouldHaveAllLevels()
    {
        // Arrange & Act
        var levels = Enum.GetValues<EducationLevel>();

        // Assert
        levels.Should().Contain(EducationLevel.HighSchool);
        levels.Should().Contain(EducationLevel.Technical);
        levels.Should().Contain(EducationLevel.Technologist);
        levels.Should().Contain(EducationLevel.Professional);
        levels.Should().Contain(EducationLevel.Specialization);
        levels.Should().Contain(EducationLevel.Master);
        levels.Should().Contain(EducationLevel.Doctorate);
    }

    [Fact]
    public void EducationLevel_ShouldHaveCorrectOrder()
    {
        // Assert - Verificar que el orden sea correcto (de menor a mayor nivel educativo)
        ((int)EducationLevel.HighSchool).Should().BeLessThan((int)EducationLevel.Technical);
        ((int)EducationLevel.Technical).Should().BeLessThan((int)EducationLevel.Technologist);
        ((int)EducationLevel.Technologist).Should().BeLessThan((int)EducationLevel.Professional);
        ((int)EducationLevel.Professional).Should().BeLessThan((int)EducationLevel.Specialization);
        ((int)EducationLevel.Specialization).Should().BeLessThan((int)EducationLevel.Master);
        ((int)EducationLevel.Master).Should().BeLessThan((int)EducationLevel.Doctorate);
    }

    [Theory]
    [InlineData(EmployeeStatus.Active, "Active")]
    [InlineData(EmployeeStatus.Inactive, "Inactive")]
    [InlineData(EmployeeStatus.Vacation, "Vacation")]
    public void EmployeeStatus_ShouldHaveCorrectNames(EmployeeStatus status, string expectedName)
    {
        // Act
        var name = status.ToString();

        // Assert
        name.Should().Be(expectedName);
    }

    [Theory]
    [InlineData(EducationLevel.HighSchool, "HighSchool")]
    [InlineData(EducationLevel.Technical, "Technical")]
    [InlineData(EducationLevel.Technologist, "Technologist")]
    [InlineData(EducationLevel.Professional, "Professional")]
    [InlineData(EducationLevel.Specialization, "Specialization")]
    [InlineData(EducationLevel.Master, "Master")]
    [InlineData(EducationLevel.Doctorate, "Doctorate")]
    public void EducationLevel_ShouldHaveCorrectNames(EducationLevel level, string expectedName)
    {
        // Act
        var name = level.ToString();

        // Assert
        name.Should().Be(expectedName);
    }

    [Fact]
    public void EmployeeStatus_CanBeParsed_FromString()
    {
        // Act
        var parsed = Enum.Parse<EmployeeStatus>("Active");

        // Assert
        parsed.Should().Be(EmployeeStatus.Active);
    }

    [Fact]
    public void EducationLevel_CanBeParsed_FromString()
    {
        // Act
        var parsed = Enum.Parse<EducationLevel>("Professional");

        // Assert
        parsed.Should().Be(EducationLevel.Professional);
    }

    [Fact]
    public void EmployeeStatus_ShouldHaveThreeValues()
    {
        // Act
        var statuses = Enum.GetValues<EmployeeStatus>();

        // Assert
        statuses.Should().HaveCount(3);
    }

    [Fact]
    public void EducationLevel_ShouldHaveSevenValues()
    {
        // Act
        var levels = Enum.GetValues<EducationLevel>();

        // Assert
        levels.Should().HaveCount(7);
    }
}

