using System.Net.Mail;
using Microsoft.Extensions.Options;
using MimeKit;
using Payment.Application.Contracts.Infrastructure;
using Payment.Application.Models;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;


namespace Payment.Infrastructure.Mail;

public class EmailService : IEmailService
{
    
    private readonly EmailSettings _mailSettings;

    public EmailService(IOptions<EmailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value ?? throw new ArgumentNullException(nameof(mailSettings));
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
            await smtp.SendAsync(email);
        }
        catch (Exception e)
        {
            return false;
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }

        return true;
    }
}