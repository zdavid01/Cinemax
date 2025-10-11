namespace Payment.API.DTOs;

/// <summary>
/// Request DTO for executing a PayPal payment after user approval
/// </summary>
public class ExecutePaymentRequestDto
{
    public string PaymentId { get; set; } = string.Empty;
    public string PayerId { get; set; } = string.Empty;
}

