using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using EmployeeManagementSystem.Domain.Interfaces;
using EmployeeManagementSystem.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManagementSystem.Tests.UnitTests;

/// <summary>
/// Pruebas unitarias para EmployeeService
/// </summary>
public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
    private readonly Mock<IJobPositionRepository> _jobPositionRepositoryMock;
    private readonly Mock<IExcelService> _excelServiceMock;
    private readonly Mock<ILogger<EmployeeService>> _loggerMock;
    private readonly EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _departmentRepositoryMock = new Mock<IDepartmentRepository>();
        _jobPositionRepositoryMock = new Mock<IJobPositionRepository>();
        _excelServiceMock = new Mock<IExcelService>();
        _loggerMock = new Mock<ILogger<EmployeeService>>();

        _employeeService = new EmployeeService(
            _employeeRepositoryMock.Object,
            _departmentRepositoryMock.Object,
            _jobPositionRepositoryMock.Object,
            _excelServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAllEmployeesAsync_ShouldReturnAllEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            CreateTestEmployee("123456", "Juan", "Pérez"),
            CreateTestEmployee("789012", "María", "González")
        };

        var jobPosition = new JobPosition("Desarrollador");
        
        _employeeRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(employees);

        _jobPositionRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(jobPosition);

        // Act
        var result = await _employeeService.GetAllEmployeesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _employeeRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_WithValidId_ShouldReturnEmployee()
    {
        // Arrange
        var employeeId = 1;
        var employee = CreateTestEmployee("123456", "Juan", "Pérez");
        var jobPosition = new JobPosition("Desarrollador");

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        _jobPositionRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(jobPosition);

        // Act
        var result = await _employeeService.GetEmployeeByIdAsync(employeeId);

        // Assert
        result.Should().NotBeNull();
        result!.Document.Should().Be("123456");
        result.FirstName.Should().Be("Juan");
        result.LastName.Should().Be("Pérez");
        _employeeRepositoryMock.Verify(x => x.GetByIdAsync(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = 999;
        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(invalidId))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _employeeService.GetEmployeeByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
        _employeeRepositoryMock.Verify(x => x.GetByIdAsync(invalidId), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeByDocumentAsync_WithValidDocument_ShouldReturnEmployee()
    {
        // Arrange
        var document = "123456";
        var employee = CreateTestEmployee(document, "Juan", "Pérez");
        var jobPosition = new JobPosition("Desarrollador");

        _employeeRepositoryMock
            .Setup(x => x.GetByDocumentIdAsync(document))
            .ReturnsAsync(employee);

        _jobPositionRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(jobPosition);

        // Act
        var result = await _employeeService.GetEmployeeByDocumentAsync(document);

        // Assert
        result.Should().NotBeNull();
        result!.Document.Should().Be(document);
        _employeeRepositoryMock.Verify(x => x.GetByDocumentIdAsync(document), Times.Once);
    }

    [Fact]
    public async Task CreateEmployeeAsync_WithValidData_ShouldCreateEmployee()
    {
        // Arrange
        var createDto = new CreateEmployeeDto
        {
            Document = "123456",
            FirstName = "Juan",
            LastName = "Pérez",
            BirthDate = new DateTime(1990, 5, 15),
            Address = "Calle 123",
            Email = "juan@example.com",
            Phone = "3001234567",
            JobPositionId = 1,
            Salary = 3500000,
            HiringDate = DateTime.Now,
            Status = 1, // Active
            EducationLevel = 3, // Professional
            ProfessionalProfile = "Desarrollador",
            DepartmentId = 1
        };

        _employeeRepositoryMock
            .Setup(x => x.GetByDocumentIdAsync(createDto.Document))
            .ReturnsAsync((Employee?)null);

        _employeeRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Employee>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _employeeService.CreateEmployeeAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Document.Should().Be(createDto.Document);
        result.FirstName.Should().Be(createDto.FirstName);
        result.LastName.Should().Be(createDto.LastName);
        _employeeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Once);
    }

    [Fact]
    public async Task CreateEmployeeAsync_WithDuplicateDocument_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateEmployeeDto
        {
            Document = "123456",
            FirstName = "Juan",
            LastName = "Pérez",
            BirthDate = new DateTime(1990, 5, 15),
            Address = "Calle 123",
            Email = "juan@example.com",
            Phone = "3001234567",
            JobPositionId = 1,
            Salary = 3500000,
            HiringDate = DateTime.Now,
            Status = 1,
            EducationLevel = 3,
            ProfessionalProfile = "Desarrollador",
            DepartmentId = 1
        };

        var existingEmployee = CreateTestEmployee(createDto.Document, "Otro", "Empleado");
        
        _employeeRepositoryMock
            .Setup(x => x.GetByDocumentIdAsync(createDto.Document))
            .ReturnsAsync(existingEmployee);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _employeeService.CreateEmployeeAsync(createDto)
        );
        
        _employeeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task DeleteEmployeeAsync_WithValidId_ShouldDeleteEmployee()
    {
        // Arrange
        var employeeId = 1;

        _employeeRepositoryMock
            .Setup(x => x.DeleteAsync(employeeId))
            .Returns(Task.CompletedTask);

        // Act
        await _employeeService.DeleteEmployeeAsync(employeeId);

        // Assert
        _employeeRepositoryMock.Verify(x => x.DeleteAsync(employeeId), Times.Once);
    }


    // Helper method para crear empleados de prueba
    private Employee CreateTestEmployee(string document, string firstName, string lastName)
    {
        return new Employee(
            document: document,
            fullName: new FullName(firstName, lastName),
            birthDate: new DateTime(1990, 1, 1),
            address: "Test Address",
            contactInfo: new ContactInfo("test@example.com", "3001234567"),
            jobInfo: new JobInfo(1, 3500000, DateTime.Now, EmployeeStatus.Active),
            educationInfo: new EducationInfo(EducationLevel.Professional),
            professionalProfile: "Test Profile",
            departmentId: 1
        );
    }
}

