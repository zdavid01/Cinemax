using System.Transactions;
using Payment.Domain.Common;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Aggregates;

public class Payment : AggregateRoot
{
    public string BuyerId { get; set; }
    public string BuyerUsername {get; set;}
    public DateTime PaymentDate {get; set;}
    public Money Money {get; set;}
    
    private readonly List<PaymentItem> _paymentItems = new List<PaymentItem>();
    
    public IReadOnlyCollection<PaymentItem> PaymentItems => _paymentItems.AsReadOnly();

    public Payment(string buyerId, string buyerUsername, DateTime paymentDate, Money money)
    {
        BuyerId = buyerId ?? throw new ArgumentNullException(nameof(buyerId));
        BuyerUsername = buyerUsername ?? throw new ArgumentNullException(nameof(buyerUsername));
        PaymentDate = paymentDate;
        Money = money ?? throw new ArgumentNullException(nameof(money));
    }

    public Payment(int id, string buyerId, string buyerUsername, DateTime paymentDate, Money money)
    {
        Id = id;
    }

    public Payment(int id)
    {
        Id = id;
    }

    public Payment(string buyerId, string buyerUsername, string currency)
    {
        BuyerId = buyerId ?? throw new ArgumentNullException(nameof(buyerId));
        BuyerUsername = buyerUsername ?? throw new ArgumentNullException(nameof(buyerUsername));
        Money = new Money(GetTotal(), currency) ?? throw new ArgumentNullException(nameof(currency));
    }

    //check if that item exists, if it does only increase quantity
    public void AddPaymentItem(PaymentItem paymentItem)
    {
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