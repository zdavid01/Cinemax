using PaymentTest.API.Services.Email.Contracts;

namespace PaymentTest.API.Services.Email.Persistance;

public interface IMailService
{
    Task SendEmailAsync(SendEmailRequest request);
}