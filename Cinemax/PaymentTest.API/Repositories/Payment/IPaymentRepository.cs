using PaymentTest.API.Data.DTOs;

namespace PaymentTest.API.Repositories.Payment;

public interface IPaymentRepository
{
    Task<IEnumerable<Entities.Payment>> GetPaymentsByUsername(string username);
    
    
}