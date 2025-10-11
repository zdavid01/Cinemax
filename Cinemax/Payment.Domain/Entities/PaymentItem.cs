using Payment.Domain.Common;
using Payment.Domain.Exceptions;

namespace Payment.Domain.Entities;

public class PaymentItem : EntityBase
{
    // Encapsulated properties - private setters
    public string MovieName { get; private set; }
    public string MovieId { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    // Private constructor for EF Core
    private PaymentItem()
    {
        MovieName = string.Empty;
        MovieId = string.Empty;
    }

    // Factory method for creating payment items
    public static PaymentItem Create(string movieName, string movieId, decimal price, int quantity)
    {
        // Guard clauses
        if (string.IsNullOrWhiteSpace(movieName))
            throw new ArgumentNullException(nameof(movieName));
        if (string.IsNullOrWhiteSpace(movieId))
            throw new ArgumentNullException(nameof(movieId));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));
        if (quantity <= 0)
            throw new PaymentDomainException("Quantity must be greater than 0");

        return new PaymentItem
        {
            MovieName = movieName,
            MovieId = movieId,
            Price = price,
            Quantity = quantity
        };
    }

    // Public constructor kept for backward compatibility (Dapper, Factory)
    public PaymentItem(string movieName, string movieId, decimal price, int quantity)
    {
        MovieName = movieName ?? throw new ArgumentNullException(nameof(movieName));
        MovieId = movieId ?? throw new ArgumentNullException(nameof(movieId));
        Price = price;
        AddQuantity(quantity);
    }

    /// <summary>
    /// Adds quantity to this payment item. Must result in positive quantity.
    /// </summary>
    public void AddQuantity(int quantity)
    {
        var newQuantity = Quantity + quantity;
        if (newQuantity <= 0)
        {
            throw new PaymentDomainException("Invalid number of quantities for payment item");
        }
        
        Quantity = newQuantity;
    }

    /// <summary>
    /// Calculates the total price for this item (Price * Quantity)
    /// </summary>
    public decimal TotalPrice()
    {
        return Price * Quantity;
    }
}