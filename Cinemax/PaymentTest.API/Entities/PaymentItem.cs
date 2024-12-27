namespace PaymentTest.API.Entities;

public class PaymentItem
{
    //base
    public int Id { get; protected set; }
    public string CreatedBy { get; private set; } = "admin";
    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    // public string LastModifiedBy { get; set; }
    // public DateTime? LastModifiedDate { get; set; }
    
    //main attributes
    public string UserId { get; set; }
    public string MovieName { get; private set; }
    public string MovieId { get; private set; }
    // public string PictureUrl { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; } = 0;

    public void AddQuantity(int quantity)
    {
        var newQuantity = quantity + Quantity;
        if (newQuantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero");
        }
        Quantity = newQuantity;
    }
}