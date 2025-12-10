using EmployeeManagementSystem.Application.DTOs;
using FluentValidation;

namespace EmployeeManagementSystem.Application.Validators;

// Validator for CreateEmployeeDto
public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("The document is mandatory")
            .MaximumLength(20).WithMessage("The document cannot exceed 20 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("The name is required")
            .MaximumLength(100).WithMessage("The name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("The last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("The email format is not valid")
            .MaximumLength(100).WithMessage("The email cannot exceed 100 characters");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("The telephone is mandatory")
            .MaximumLength(20).WithMessage("The phone cannot exceed 20 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is mandatory")
            .MaximumLength(200).WithMessage("The address cannot exceed 200 characters");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Date of birth is mandatory")
            .LessThan(DateTime.Today).WithMessage("Date of birth must be before today")
            .GreaterThan(DateTime.Today.AddYears(-100)).WithMessage("Date of birth is not valid");

        RuleFor(x => x.HiringDate)
            .NotEmpty().WithMessage("Hiring date is mandatory")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("The hiring date cannot be in the future");

        RuleFor(x => x.Salary)
            .GreaterThan(0).WithMessage("The salary must be greater than 0");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("You must select a valid department");


        RuleFor(x => x.ProfessionalProfile)
            .MaximumLength(1000).WithMessage("The professional profile cannot exceed 1000 characters");
    }
}

