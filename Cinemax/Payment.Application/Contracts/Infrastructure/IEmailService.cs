using Payment.Application.Models;

namespace Payment.Application.Contracts.Infrastructure;

public interface IEmailService
{
    Task<bool> SendEmail(Email emailRequest);
}