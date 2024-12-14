namespace Payment.API.Entities;

public class PaymentItem
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Moviename { get; set; }
    public string MovieId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; private set; }

    public PaymentItem(string moviename, string movieId, decimal price, int quantity)
    {
        CreatedAt = DateTime.Now;
        Moviename = moviename ?? throw new ArgumentNullException(nameof(moviename));
        MovieId = movieId ?? throw new ArgumentNullException(nameof(MovieId));
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