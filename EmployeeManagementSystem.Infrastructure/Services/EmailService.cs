using EmployeeManagementSystem.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace EmployeeManagementSystem.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // Send a welcome email to a new employee
    public async Task SendWelcomeEmailAsync(string toEmail, string employeeName)
    {
        var subject = "Welcome to TalentoPlus S.A.S";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 8px; padding: 20px;'>
                    <h2 style='color: #2c3e50;'>Welcome to TalentoPlus S.A.S!</h2>
                    <p>Dear <strong>{employeeName}</strong>,</p>
                    <p>We are pleased to inform you that your registration on our platform has been successfully completed.</p>
                    <p>From now on, you are part of our work team.</p>
                    <p>Your credentials have been set up and you will soon receive more information about next steps and access to our systems..</p>
                    <br/>
                    <p>kind regards,</p>
                    <p><strong>Human Resources Team</strong><br/>
                    TalentoPlus S.A.S</p>
                    <hr style='border: none; border-top: 1px solid #ddd; margin-top: 20px;'/>
                    <p style='font-size: 12px; color: #7f8c8d;'>This is an automated email, please do not respond.</p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    // General method to send an email
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var senderName = _configuration["EmailSettings:SenderName"];
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];
        var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            // Connect to SMTP server
            await client.ConnectAsync(smtpServer, smtpPort, enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            
            // Authenticate
            await client.AuthenticateAsync(username, password);
            
            // Send the message
            await client.SendAsync(message);
            
            _logger.LogInformation("Email successfully sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending mail to {Email}", toEmail);
            throw new Exception($"Error sending mail: {ex.Message}", ex);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
