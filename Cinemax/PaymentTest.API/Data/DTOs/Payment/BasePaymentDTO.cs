namespace PaymentTest.API.Data.DTOs.Payment;

public class BasePaymentDTO
{
    //payment information
    public string UserId { get; set; }
    public string Username { get; set; }
    public string UserEmail { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public IEnumerable<PaymentItemDTO> PaymentItems { get; set; }
}