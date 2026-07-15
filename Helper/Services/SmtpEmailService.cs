using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using StudentManagement.Services;
using StudentManagement.Domain.Models;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }
    /// <summary>
    /// sets up MimeMessage, and place sending email, sending to person, and body and subject
    /// of email =
    /// </summary>
    /// <param name="toEmail">address of person to which sending</param>
    /// <param name="subject">The Subject of email</param>
    /// <param name="htmlBody">Message inside the email</param>
    /// <returns>thrown exception if not sent successfully</returns>

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using SmtpClient client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}