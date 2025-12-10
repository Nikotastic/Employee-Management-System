using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using EmployeeManagementSystem.Domain.ValueObjects;
using EmployeeManagementSystem.Infrastructure.Data;
using EmployeeManagementSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Tests.IntegrationTests;

/// <summary>
/// Pruebas de integración para el EmployeeRepository
/// </summary>
public class EmployeeRepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EmployeeRepository _repository;

    public EmployeeRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EmployeeRepository(_context);

        // Seed initial data
        SeedData();
    }

    private void SeedData()
    {
        var department = new Department("Tecnología");
        _context.Departments.Add(department);

        var jobPosition = new JobPosition("Desarrollador");
        _context.JobPositions.Add(jobPosition);

        _context.SaveChanges();
    }

    [Fact]
    public async Task AddAsync_ShouldAddEmployeeToDatabase()
    {
        // Arrange
        var employee = CreateTestEmployee("123456", "Juan", "Pérez");

        // Act
        await _repository.AddAsync(employee);
        var result = await _repository.GetByDocumentIdAsync("123456");

        // Assert
        result.Should().NotBeNull();
        result!.Document.Should().Be("123456");
        result.FullName.FirstName.Should().Be("Juan");
        result.FullName.LastName.Should().Be("Pérez");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEmployees()
    {
        // Arrange
        var employee1 = CreateTestEmployee("111111", "Juan", "Pérez");
        var employee2 = CreateTestEmployee("222222", "María", "González");
        
        await _repository.AddAsync(employee1);
        await _repository.AddAsync(employee2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnEmployee()
    {
        // Arrange
        var employee = CreateTestEmployee("333333", "Carlos", "López");
        await _repository.AddAsync(employee);
        
        var allEmployees = await _repository.GetAllAsync();
        var addedEmployee = allEmployees.First(e => e.Document == "333333");

        // Act
        var result = await _repository.GetByIdAsync(addedEmployee.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Document.Should().Be("333333");
        result.FullName.FirstName.Should().Be("Carlos");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(99999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByDocumentIdAsync_WithValidDocument_ShouldReturnEmployee()
    {
        // Arrange
        var employee = CreateTestEmployee("444444", "Ana", "Martínez");
        await _repository.AddAsync(employee);

        // Act
        var result = await _repository.GetByDocumentIdAsync("444444");

        // Assert
        result.Should().NotBeNull();
        result!.Document.Should().Be("444444");
        result.FullName.FirstName.Should().Be("Ana");
    }

    [Fact]
    public async Task GetByDocumentIdAsync_WithInvalidDocument_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByDocumentIdAsync("INVALID_DOC");

        // Assert
        result.Should().BeNull();
    }


    [Fact]
    public async Task DeleteAsync_ShouldRemoveEmployeeFromDatabase()
    {
        // Arrange
        var employee = CreateTestEmployee("666666", "Laura", "Fernández");
        await _repository.AddAsync(employee);
        
        var addedEmployee = await _repository.GetByDocumentIdAsync("666666");
        addedEmployee.Should().NotBeNull();

        // Act
        await _repository.DeleteAsync(addedEmployee!.Id);
        var result = await _repository.GetByDocumentIdAsync("666666");

        // Assert
        result.Should().BeNull();
    }


    private Employee CreateTestEmployee(string document, string firstName, string lastName)
    {
        return new Employee(
            document: document,
            fullName: new FullName(firstName, lastName),
            birthDate: new DateTime(1990, 1, 1),
            address: "Test Address 123",
            contactInfo: new ContactInfo($"{firstName.ToLower()}@example.com", "3001234567"),
            jobInfo: new JobInfo(1, 3500000, DateTime.Now, EmployeeStatus.Active),
            educationInfo: new EducationInfo(EducationLevel.Professional),
            professionalProfile: "Professional profile",
            departmentId: 1
        );
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

