namespace EmployeeManagementSystem.Application.Interfaces;

// Service for sending emails
public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string employeeName);
    Task SendEmailAsync(string toEmail, string subject, string body);
}

