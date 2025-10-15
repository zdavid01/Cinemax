namespace Payment.API.DTOs;

/// <summary>
/// Response DTO for payment execution result
/// </summary>
public class ExecutePaymentResponseDto
{
    public string State { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

