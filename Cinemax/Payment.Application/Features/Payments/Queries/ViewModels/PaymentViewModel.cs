namespace Payment.Application.Features.Payments.Queries.ViewModels;

public class PaymentViewModel
{
    // Relevant information from EntityBase
    public int Id { get; set; }
    
    //relevant information from Money
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    
    //relevant information from Payment
    public string BuyerId { get; set; }
    public string BuyerUsername { get; set; }
    public decimal TotalPrice { get; set; }
    public IEnumerable<PaymentItemViewModel> PaymentItems { get; set; }
}