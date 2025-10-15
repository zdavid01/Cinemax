using Email.API.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Email.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                var email = new MimeMessage();
                
                // From
                email.From.Add(new MailboxAddress(
                    _emailSettings.FromName, 
                    emailRequest.From ?? _emailSettings.FromEmail));

                // To
                email.To.Add(MailboxAddress.Parse(emailRequest.To));

                // CC
                foreach (var cc in emailRequest.Cc)
                {
                    email.Cc.Add(MailboxAddress.Parse(cc));
                }

                // BCC
                foreach (var bcc in emailRequest.Bcc)
                {
                    email.Bcc.Add(MailboxAddress.Parse(bcc));
                }

                // Subject
                email.Subject = emailRequest.Subject;

                // Body
                var bodyBuilder = new BodyBuilder();
                if (emailRequest.IsHtml)
                {
                    bodyBuilder.HtmlBody = emailRequest.Body;
                }
                else
                {
                    bodyBuilder.TextBody = emailRequest.Body;
                }

                // Attachments
                foreach (var attachment in emailRequest.Attachments)
                {
                    if (File.Exists(attachment.Value))
                    {
                        bodyBuilder.Attachments.Add(attachment.Value);
                    }
                }

                email.Body = bodyBuilder.ToMessageBody();

                // Priority
                email.Priority = emailRequest.Priority switch
                {
                    2 => MessagePriority.Urgent,
                    0 => MessagePriority.NonUrgent,
                    _ => MessagePriority.Normal
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, 
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                
                await smtp.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {To}", emailRequest.To);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", emailRequest.To);
                return false;
            }
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            var emailRequest = new EmailRequest
            {
                To = to,
                Subject = subject,
                Body = body,
                IsHtml = isHtml
            };

            return await SendEmailAsync(emailRequest);
        }
    }
}


