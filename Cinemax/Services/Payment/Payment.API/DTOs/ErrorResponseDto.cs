namespace Payment.API.DTOs;

/// <summary>
/// Standard error response DTO
/// </summary>
public class ErrorResponseDto
{
    public string Error { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

