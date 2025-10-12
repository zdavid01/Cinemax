using Payment.Application.Contracts.Infrastructure;
using StackExchange.Redis;
using System.Text.Json;

namespace Payment.Infrastructure.Services;

/// <summary>
/// Service for managing baskets using Redis cache
/// </summary>
public class BasketService : IBasketService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<BasketService> _logger;

    public BasketService(IConnectionMultiplexer redis, ILogger<BasketService> logger)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<BasketData?> GetBasketAsync(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            var db = _redis.GetDatabase();
            var basketKey = username;
            
            // Get basket data from Redis hash (stored by IDistributedCache)
            var basketData = await db.HashGetAsync(basketKey, "data");

            if (basketData.IsNullOrEmpty)
            {
                _logger.LogInformation("No basket found for user {Username}", username);
                return null;
            }

            // Deserialize basket data with case-insensitive property names
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };
            var basket = JsonSerializer.Deserialize<BasketData>(basketData!, options);
            _logger.LogInformation("Retrieved basket with {ItemCount} items for user {Username}", 
                basket?.Items?.Count ?? 0, username);

            return basket;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving basket for user {Username}", username);
            throw;
        }
    }

    public async Task ClearBasketAsync(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(username);
            
            _logger.LogInformation("Basket cleared for user {Username}", username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing basket for user {Username}", username);
            throw;
        }
    }

    public async Task StorePaymentEmailAsync(string paymentId, string email, TimeSpan expiry)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                throw new ArgumentException("PaymentId cannot be null or empty", nameof(paymentId));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            var db = _redis.GetDatabase();
            var key = $"payment:email:{paymentId}";
            
            await db.StringSetAsync(key, email, expiry);
            _logger.LogInformation("Stored email {Email} for payment {PaymentId}", email, paymentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing email for payment {PaymentId}", paymentId);
            throw;
        }
    }

    public async Task<string?> GetPaymentEmailAsync(string paymentId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                throw new ArgumentException("PaymentId cannot be null or empty", nameof(paymentId));

            var db = _redis.GetDatabase();
            var key = $"payment:email:{paymentId}";
            
            var email = await db.StringGetAsync(key);
            
            if (!email.IsNullOrEmpty)
            {
                // Delete the key after retrieval (one-time use)
                await db.KeyDeleteAsync(key);
                _logger.LogInformation("Retrieved and deleted email for payment {PaymentId}", paymentId);
                return email.ToString();
            }

            _logger.LogWarning("No email found for payment {PaymentId}", paymentId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving email for payment {PaymentId}", paymentId);
            throw;
        }
    }
}

