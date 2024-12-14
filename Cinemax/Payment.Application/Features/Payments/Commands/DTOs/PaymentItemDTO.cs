namespace Payment.Application.Features.Payments.Commands.DTOs;

public class PaymentItemDTO
{
    // Relevant information from PaymentItem
    public string MovieName { get; set; }
    public string MovieId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}