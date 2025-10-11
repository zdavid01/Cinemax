namespace Payment.API.DTOs;

/// <summary>
/// Request DTO for initiating a PayPal payment
/// </summary>
public class PaymentRequestDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

