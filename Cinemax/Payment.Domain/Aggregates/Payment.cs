using System.Transactions;
using Payment.Domain.Common;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Aggregates;

public class Payment : AggregateRoot
{
    // Encapsulated properties - private setters
    public string BuyerId { get; private set; }
    public string BuyerUsername { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public Money Money { get; private set; }
    
    private readonly List<PaymentItem> _paymentItems = new List<PaymentItem>();
    
    public IReadOnlyCollection<PaymentItem> PaymentItems => _paymentItems.AsReadOnly();

    // Private constructor for EF Core
    private Payment()
    {
        BuyerId = string.Empty;
        BuyerUsername = string.Empty;
        Money = new Money(0, "USD");
    }

    // Factory method - preferred way to create a Payment
    public static Payment Create(string buyerId, string buyerUsername, Money money)
    {
        // Guard clauses
        if (string.IsNullOrWhiteSpace(buyerId))
            throw new ArgumentNullException(nameof(buyerId));
        if (string.IsNullOrWhiteSpace(buyerUsername))
            throw new ArgumentNullException(nameof(buyerUsername));
        if (money == null)
            throw new ArgumentNullException(nameof(money));

        var payment = new Payment
        {
            BuyerId = buyerId,
            BuyerUsername = buyerUsername,
            PaymentDate = DateTime.UtcNow,
            Money = money
        };

        return payment;
    }

    // Backward compatibility constructor (still used by factory)
    public Payment(string buyerId, string buyerUsername, DateTime paymentDate, Money money)
    {
        BuyerId = buyerId ?? throw new ArgumentNullException(nameof(buyerId));
        BuyerUsername = buyerUsername ?? throw new ArgumentNullException(nameof(buyerUsername));
        PaymentDate = paymentDate;
        Money = money ?? throw new ArgumentNullException(nameof(money));
    }

    // Legacy constructors kept for backward compatibility
    public Payment(int id, string buyerId, string buyerUsername, DateTime paymentDate, Money money)
    {
        Id = id;
        BuyerId = buyerId;
        BuyerUsername = buyerUsername;
        PaymentDate = paymentDate;
        Money = money;
    }

    public Payment(int id)
    {
        Id = id;
        BuyerId = string.Empty;
        BuyerUsername = string.Empty;
        Money = new Money(0, "USD");
    }

    public Payment(string buyerId, string buyerUsername, string currency)
    {
        BuyerId = buyerId ?? throw new ArgumentNullException(nameof(buyerId));
        BuyerUsername = buyerUsername ?? throw new ArgumentNullException(nameof(buyerUsername));
        Money = new Money(GetTotal(), currency) ?? throw new ArgumentNullException(nameof(currency));
    }

    /// <summary>
    /// Adds a payment item to this payment. If item for same movie exists, increases quantity.
    /// </summary>
    public void AddPaymentItem(PaymentItem paymentItem)
    {
        // Guard clause
        if (paymentItem == null)
            throw new ArgumentNullException(nameof(paymentItem));

        var existingPaymentForMovie = PaymentItems.SingleOrDefault(p => p.MovieId == paymentItem.MovieId);

        if (existingPaymentForMovie is null)
        {
            _paymentItems.Add(paymentItem);
        }
        else
        {
            existingPaymentForMovie.AddQuantity(paymentItem.Quantity);
        }
    }
    
    //todo different currencies?
    public decimal GetTotal() => PaymentItems.Sum(paymentItem => paymentItem.Quantity * paymentItem.Price);
    
    public void RecalculateTotal(string currency)
    {
        Money = new Money(GetTotal(), currency);
    }
    
}