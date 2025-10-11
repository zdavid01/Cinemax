namespace Payment.API.DTOs;

/// <summary>
/// Response DTO for PayPal payment creation
/// </summary>
public class PaymentResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public List<PaymentLinkDto> Links { get; set; } = new();
}

/// <summary>
/// Payment link DTO (e.g., approval URL)
/// </summary>
public class PaymentLinkDto
{
    public string Href { get; set; } = string.Empty;
    public string Rel { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
}

