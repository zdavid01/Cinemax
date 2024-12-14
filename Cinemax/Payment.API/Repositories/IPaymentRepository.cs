using Payment.API.DTOs;


namespace Payment.API.Repositories;

public interface IPaymentRepository
{
    Task<PaymentItemDTO?> GetPaymentItem(int paymentId);
    Task<bool> CreatePaymentItem(CreatePaymentItemDTO paymentItem);
    Task<bool> DeletePaymentItem(int paymentItemId);
    Task<IEnumerable<PaymentItemDTO>> GetPaymentItemsForMovie(string movieId);
}