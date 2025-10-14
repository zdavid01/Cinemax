using Email.API.Models;

namespace Email.API.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailRequest emailRequest);
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    }
}


