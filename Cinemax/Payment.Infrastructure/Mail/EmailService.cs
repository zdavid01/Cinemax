using System.Net.Mail;
using Microsoft.Extensions.Options;
using MimeKit;
using Payment.Application.Contracts.Infrastructure;
using Payment.Application.Models;
using MailKit.Net.Smtp;
using Org.BouncyCastle.Tls;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;


namespace Payment.Infrastructure.Mail;

public class EmailService : IEmailService
{
    
    private readonly EmailSettings _mailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> mailSettings, ILogger<EmailService> logger)
    {
        _mailSettings = mailSettings.Value ?? throw new ArgumentNullException(nameof(mailSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<bool> SendEmail(Email emailRequest)
    {
        var email = new MimeMessage();
        
        email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
        email.To.Add(MailboxAddress.Parse(emailRequest.To));
        email.Subject = emailRequest.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = emailRequest.Body,
            TextBody = emailRequest.Body
        };
        email.Body = builder.ToMessageBody();
        
        //connection
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);

        try
        {
            _logger.LogInformation($"Sending email via SMTP server {_mailSettings.Host} to: {emailRequest.To}");
            await smtp.SendAsync(email);
        }
        catch (Exception e)
        {
            _logger.LogInformation($"An error had occured when sending email via SMTP server {_mailSettings.Host}: {e.Message}");
            return false;
        }
        finally
        {
            
            await smtp.DisconnectAsync(true);
        }

        return true;
    }
}