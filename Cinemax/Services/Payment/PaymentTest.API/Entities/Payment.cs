namespace PaymentTest.API.Entities;

public class Payment
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public string UserEmail { get; set; }
    public DateTime PaymentDate { get; set; }
    
    //DDD
    private readonly List<PaymentItem> _paymentItems = new List<PaymentItem>();
    public IReadOnlyCollection<PaymentItem> PaymentItems => _paymentItems;
    
    public Payment(string userId, string username, string userEmail, DateTime paymentDate)
    {
        userId = userId ?? throw new ArgumentNullException(nameof(userId));
        username = username ?? throw new ArgumentNullException(nameof(username));
        userEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        paymentDate = DateTime.Now;
    }
    
    public Payment(int id, string userId, string username, string userEmail, DateTime paymentDate)
        :this(userId, username, userEmail, paymentDate)
    {
        Id = id;
    }
    
    public Payment(int id)
    {
        Id = id;
    }

    public void AddPaymentItem(PaymentItem paymentItem)
    {
        var existingPaymentItem = PaymentItems.SingleOrDefault(p => p.MovieId == paymentItem.MovieId);

        if (existingPaymentItem != null)
        {
            _paymentItems.Add(paymentItem);
        }
        else
        {
            existingPaymentItem.AddQuantity(paymentItem.Quantity);
        }
    }

    public decimal GetTotal()
    {
        return PaymentItems.Sum(p => p.Price * p.Quantity);
    }
    
}