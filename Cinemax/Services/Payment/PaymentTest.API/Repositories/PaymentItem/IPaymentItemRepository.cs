using PaymentTest.API.Data.DTOs;

namespace PaymentTest.API.Repositories;

public interface IPaymentItemRepository
{
    Task<List<PaymentItemDTO>> GetPaymentItems();
    Task<PaymentItemDTO> GetPaymentItemByMovieId(string movieName);
    Task<bool> CreatePaymentItem(CreatePaymentItemDTO paymentItemDTO);
    
    Task<bool> UpdatePaymentItem(UpdatePaymentItemDTO paymentItem);
    Task<bool> DeletePaymentItem(int id);
    
    Task<IEnumerable<PaymentItemDTO>> GetPaymentItemsByMovieId(string movieId);
    Task<IEnumerable<PaymentItemDTO>> GetPaymentItemsByUserId(string userId);
}