using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace EmployeeManagementSystem.Tests.UnitTests;

/// <summary>
/// Pruebas unitarias para los validadores de DTOs
/// </summary>
public class ValidatorsTests
{
    private readonly CreateEmployeeDtoValidator _createEmployeeValidator;
    private readonly LoginDtoValidator _loginValidator;

    public ValidatorsTests()
    {
        _createEmployeeValidator = new CreateEmployeeDtoValidator();
        _loginValidator = new LoginDtoValidator();
    }

    #region CreateEmployeeDtoValidator Tests

    [Fact]
    public void CreateEmployeeValidator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            Document = "1234567890",
            FirstName = "Juan",
            LastName = "Pérez",
            BirthDate = new DateTime(1990, 5, 15),
            Address = "Calle 123 #45-67",
            Email = "juan.perez@example.com",
            Phone = "3001234567",
            JobPositionId = 1,
            Salary = 3500000,
            HiringDate = new DateTime(2023, 1, 15),
            Status = 1,
            EducationLevel = 3,
            ProfessionalProfile = "Desarrollador con experiencia",
            DepartmentId = 1
        };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateEmployeeValidator_WithEmptyDocument_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateEmployeeDto { Document = "" };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Document);
    }

    [Fact]
    public void CreateEmployeeValidator_WithEmptyFirstName_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateEmployeeDto { FirstName = "" };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void CreateEmployeeValidator_WithEmptyLastName_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateEmployeeDto { LastName = "" };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void CreateEmployeeValidator_WithInvalidEmail_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateEmployeeDto { Email = "invalid-email" };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void CreateEmployeeValidator_WithEmptyEmail_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateEmployeeDto { Email = "" };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void CreateEmployeeValidator_WithEmptyPhone_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateEmployeeDto { Phone = "" };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void CreateEmployeeValidator_WithNegativeSalary_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateEmployeeDto { Salary = -1000 };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Salary);
    }

    [Fact]
    public void CreateEmployeeValidator_WithZeroSalary_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateEmployeeDto { Salary = 0 };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Salary);
    }



    [Fact]
    public void CreateEmployeeValidator_WithFutureBirthDate_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateEmployeeDto { BirthDate = DateTime.Now.AddDays(1) };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BirthDate);
    }

    [Fact]
    public void CreateEmployeeValidator_WithFutureHiringDate_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateEmployeeDto { HiringDate = DateTime.Now.AddDays(1) };

        // Act
        var result = _createEmployeeValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HiringDate);
    }

    #endregion

    #region LoginDtoValidator Tests

    [Fact]
    public void LoginValidator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var result = _loginValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void LoginValidator_WithEmptyEmail_ShouldHaveError()
    {
        // Arrange
        var dto = new LoginDto { Email = "" };

        // Act
        var result = _loginValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void LoginValidator_WithEmptyPassword_ShouldHaveError()
    {
        // Arrange
        var dto = new LoginDto { Password = "" };

        // Act
        var result = _loginValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void LoginValidator_WithNullEmail_ShouldHaveError()
    {
        // Arrange
        var dto = new LoginDto { Email = null! };

        // Act
        var result = _loginValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void LoginValidator_WithNullPassword_ShouldHaveError()
    {
        // Arrange
        var dto = new LoginDto { Password = null! };

        // Act
        var result = _loginValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    #endregion
}

