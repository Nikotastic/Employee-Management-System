using EmployeeManagementSystem.Application.DTOs;
using FluentValidation;

namespace EmployeeManagementSystem.Application.Validators;

// Validator for LoginDto
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("The email format is invalid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("The password must be at least 6 characters long");
    }
}

