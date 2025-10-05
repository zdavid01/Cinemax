using Payment.Application.Factories;
using Payment.Application.Features.Payments.Queries.ViewModels;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Factories;

public class PaymentViewModelFactory : IPaymentViewModelFactory
{
    public PaymentViewModel CreateViewModel(Domain.Aggregates.Payment payment)
    {
        var paymentVM = new PaymentViewModel();
        paymentVM.Id = payment.Id;
        paymentVM.BuyerId = payment.BuyerId;
        paymentVM.BuyerUsername = payment.BuyerUsername;
        paymentVM.Amount = payment.Money.Amount;
        paymentVM.Currency = payment.Money.Currency;
        paymentVM.TotalPrice = payment.PaymentItems.Sum(i => i.Price * i.Quantity);
        
        var paymentItems = new List<PaymentItemViewModel>();

        foreach (var item in payment.PaymentItems)
        {
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