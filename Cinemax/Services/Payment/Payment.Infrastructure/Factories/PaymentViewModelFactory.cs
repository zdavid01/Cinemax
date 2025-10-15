using Payment.Application.Factories;
using Payment.Application.Features.Payments.Queries.ViewModels;
using Payment.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Payment.Infrastructure.Factories;

public class PaymentViewModelFactory : IPaymentViewModelFactory
{
    private readonly ILogger<PaymentViewModelFactory> _logger;

    public PaymentViewModelFactory(ILogger<PaymentViewModelFactory> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public PaymentViewModel CreateViewModel(Domain.Aggregates.Payment payment)
    {
        var paymentVM = new PaymentViewModel();
        paymentVM.Id = payment.Id;
        paymentVM.BuyerId = payment.BuyerId;
        paymentVM.BuyerUsername = payment.BuyerUsername;
        paymentVM.Amount = payment.Money.Amount;
        paymentVM.Currency = payment.Money.Currency;
        paymentVM.TotalPrice = payment.PaymentItems.Sum(i => i.Price * i.Quantity);
        
        // Add timestamp information
        paymentVM.CreatedDate = payment.CreatedDate;
        paymentVM.CreatedBy = payment.CreatedBy;
        paymentVM.PaymentDate = payment.PaymentDate;
        
        // Debug logging
        _logger.LogInformation("Payment {PaymentId}: PaymentDate={PaymentDate}, CreatedDate={CreatedDate}, CreatedBy={CreatedBy}", 
            payment.Id, payment.PaymentDate, payment.CreatedDate, payment.CreatedBy);
        
        var paymentItems = new List<PaymentItemViewModel>();

        _logger.LogInformation("Payment {PaymentId} has {ItemCount} payment items", payment.Id, payment.PaymentItems.Count);

        foreach (var item in payment.PaymentItems)
        {
            _logger.LogInformation("Processing item: {ItemId}, MovieName: {MovieName}, MovieId: {MovieId}, Price: {Price}, Quantity: {Quantity}", 
                item.Id, item.MovieName, item.MovieId, item.Price, item.Quantity);
            
            var paymentItem = new PaymentItemViewModel();
            paymentItem.Id = item.Id;
            paymentItem.MovieName = item.MovieName;
            paymentItem.MovieId = item.MovieId;
            paymentItem.Price = item.Price;
            paymentItem.Quantity = item.Quantity;
            paymentItems.Add(paymentItem);
        }
        paymentVM.PaymentItems = paymentItems;
        
        return paymentVM;
    }
}