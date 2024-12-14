namespace Payment.API.Entities;

public class PaymentItem
{
    public int Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string MovieName { get; set; }
    public string MovieId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; private set; }

    
    //todo FIX THIS DAPPER
    public PaymentItem(int id, DateTime? createdAt, string movieName, string movieId, decimal price, int quantity)
    {
        Id = id;
        CreatedAt = createdAt ?? DateTime.Now;
        MovieName = movieName ?? throw new ArgumentNullException(nameof(movieName));
        MovieId = movieId ?? throw new ArgumentNullException(nameof(movieId));
        Price = price;
        AddQuantity(quantity);
    }

    private void AddQuantity(int quantity)
    {
        var newQuantity = Quantity + quantity;
        if (newQuantity <= 0)
        {
            //todo exception
        }
        
        Quantity = newQuantity;
    }

    public decimal TotalPrice()
    {
        return Price * Quantity;
    }
}