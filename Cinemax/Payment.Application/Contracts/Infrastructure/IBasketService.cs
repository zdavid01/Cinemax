namespace Payment.Application.Contracts.Infrastructure;

/// <summary>
/// Service for managing shopping baskets
/// </summary>
public interface IBasketService
{
    /// <summary>
    /// Retrieves basket items for a user
    /// </summary>
    Task<BasketData?> GetBasketAsync(string username);
    
    /// <summary>
    /// Clears the basket for a user after successful payment
    /// </summary>
    Task ClearBasketAsync(string username);
    
    /// <summary>
    /// Stores user email temporarily for payment callback
    /// </summary>
    Task StorePaymentEmailAsync(string paymentId, string email, TimeSpan expiry);
    
    /// <summary>
    /// Retrieves stored email for payment callback
    /// </summary>
    Task<string?> GetPaymentEmailAsync(string paymentId);
}

/// <summary>
/// Represents basket data from cache
/// </summary>
public class BasketData
{
    public string? Username { get; set; }
    public List<BasketItemData> Items { get; set; } = new();
}

/// <summary>
/// Represents an item in the basket
/// </summary>
public class BasketItemData
{
    public string Title { get; set; } = string.Empty;
    public string MovieId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Rating { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

