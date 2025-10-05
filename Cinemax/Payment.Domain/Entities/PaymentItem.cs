using Payment.Domain.Common;
using Payment.Domain.Exceptions;

namespace Payment.Domain.Entities;

public class PaymentItem : EntityBase
{

    public string MovieName { get; set; }
    public string MovieId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    
    //todo FIX THIS DAPPER
    public PaymentItem(string movieName, string movieId, decimal price, int quantity)
    {
        MovieName = movieName ?? throw new ArgumentNullException(nameof(movieName));
        MovieId = movieId ?? throw new ArgumentNullException(nameof(movieId));
        Price = price;
        AddQuantity(quantity);
    }

    public void AddQuantity(int quantity)
    {
        var newQuantity = Quantity + quantity;
        if (newQuantity <= 0)
        {
            throw new PaymentDomainException("Invalid number of quantities for payment item");
        }
        
        Quantity = newQuantity;
    }

    public decimal TotalPrice()
    {
        return Price * Quantity;
    }
}