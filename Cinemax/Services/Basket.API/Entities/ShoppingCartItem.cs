namespace Basket.API.Entities;

public class ShoppingCartItem
{ 
    public decimal Price { get; set; }
    public required string MovieId { get; set; }
}   