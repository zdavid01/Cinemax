namespace Payment.API.DTOs;

public class BasePaymentItemDTO
{
    public DateTime CreatedAt { get; set; }
    public string? MovieName { get; set; }
    public string? MovieId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; private set; }
}