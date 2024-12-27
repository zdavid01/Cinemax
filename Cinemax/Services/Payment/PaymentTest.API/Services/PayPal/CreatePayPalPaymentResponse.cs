namespace PaymentTest.API.Services.PayPal;

public class CreatePayPalPaymentResponse
{
    public string? ApprovalUrl { get; set; }
    public string PaymentId { get; set; }
}