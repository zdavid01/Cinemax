using Payment.Application.Factories;
using Payment.Application.Features.Payments.Commands.CreatePayment;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Infrastructure.Factories;

public class PaymentFactory : IPaymentFactory
{
    public Domain.Aggregates.Payment Create(CreatePaymentCommand command)
    {
        // Calculate total amount from payment items
        var totalAmount = command.PaymentItems.Sum(item => item.Price * item.Quantity);
        var money = new Money(totalAmount, command.Currency);
        
        // Use constructor that sets PaymentDate to current time
        var payment = new Domain.Aggregates.Payment(
            command.BuyerId, 
            command.BuyerUsername, 
            DateTime.UtcNow, 
            money);

        foreach (var item in command.PaymentItems)
        {
            var paymentItem = new PaymentItem(item.MovieName, item.MovieId, item.Price, item.Quantity);
            payment.AddPaymentItem(paymentItem);
        }
        
        return payment;
    }
}