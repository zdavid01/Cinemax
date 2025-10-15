namespace Payment.Application.Features.Payments.Queries.ViewModels;

public class PaymentItemViewModel
{
    
    //relevant information from EntityBase
    public int Id { get; set; }
    
    //relevant information from PaymentItem
    public string MovieName { get; set; }
    public string MovieId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}