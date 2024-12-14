using Payment.Domain.Common;

namespace Payment.Domain.Entities;

public class PaymentItem : EntityBase
{

    public string MovieName { get; set; }
    public string MovieId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; private set; }

    
    //todo FIX THIS DAPPER
    public PaymentItem(string movieName, string movieId, decimal price, int quantity)
    {
        MovieName = movieName ?? throw new ArgumentNullException(nameof(movieName));
        MovieId = movieId ?? throw new ArgumentNullException(nameof(movieId));
        Price = TotalPrice();
        AddQuantity(quantity);
    }

    public void AddQuantity(int quantity)
    {
        var newQuantity = Quantity + quantity;
        if (newQuantity <= 0)
        {
            
        }
        
        Quantity = newQuantity;
    }

    public decimal TotalPrice()
    {
        return Price * Quantity;
    }
}