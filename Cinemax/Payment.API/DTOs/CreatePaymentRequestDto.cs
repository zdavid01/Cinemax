namespace Payment.API.DTOs;

/// <summary>
/// Request DTO for creating a payment in the database
/// </summary>
public class CreatePaymentRequestDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string BuyerId { get; set; } = string.Empty;
    public string BuyerUsername { get; set; } = string.Empty;
    public List<PaymentItemRequestDto> PaymentItems { get; set; } = new();
}

/// <summary>
/// DTO for payment item in request
/// </summary>
public class PaymentItemRequestDto
{
    public string MovieName { get; set; } = string.Empty;
    public string MovieId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

