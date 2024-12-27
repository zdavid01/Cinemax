using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using PaymentTest.API.Services.Email.Contracts;
using PaymentTest.API.Services.Email.Persistance;


namespace Test.Email.Services;

public class GmailService : IMailService
{

    private readonly GmailOptions _gmailOptions;
    
    public GmailService(IOptions<GmailOptions> gmailOptions)
    {
        _gmailOptions = gmailOptions.Value ?? throw new ArgumentNullException(nameof(_gmailOptions));
    }

    public async Task SendEmailAsync(SendEmailRequest request)
    {
        MailMessage mailMessage = new MailMessage()
        {
            From = new MailAddress(_gmailOptions.Email),
            Subject = request.Subject,
            Body = request.Body,
        };
        
        mailMessage.To.Add(request.To);

        using var smtpClient = new SmtpClient();
        smtpClient.Host = _gmailOptions.Host;
        smtpClient.Port = _gmailOptions.Port;
        smtpClient.Credentials = new NetworkCredential(_gmailOptions.Email, _gmailOptions.Password);
        smtpClient.EnableSsl = true;
        
        await smtpClient.SendMailAsync(mailMessage);
    }
    
}