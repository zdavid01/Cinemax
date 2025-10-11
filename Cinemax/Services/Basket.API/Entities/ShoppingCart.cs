namespace Basket.API.Entities;

public class ShoppingCart
{
    public ShoppingCart(string username = null)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
    }

    public string Username { get; set; }
    public List<ShoppingCartItem> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(i => i.Price);
}

public class ShoppingCartItem
{ 
    public decimal Price { get; set; }
    public required string MovieId { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public string Rating { get; set; }
}   

