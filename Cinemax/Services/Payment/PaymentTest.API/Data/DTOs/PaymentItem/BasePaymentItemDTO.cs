namespace PaymentTest.API.Data.DTOs;

public class BasePaymentItemDTO
{
    public string UserId { get; set; }
    public string MovieName { get; set; }
    public string MovieId { get; set; }
    // public string MovieUrl { get; private set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 0;
}