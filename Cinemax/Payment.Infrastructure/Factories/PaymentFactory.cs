using Payment.Application.Factories;
using Payment.Application.Features.Payments.Commands.CreatePayment;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Infrastructure.Factories;

public class PaymentFactory : IPaymentFactory
{
    public Domain.Aggregates.Payment Create(CreatePaymentCommand command)
    {
        var payment = new Domain.Aggregates.Payment(command.BuyerId, command.BuyerUsername,
            command.Currency);

        foreach (var item in command.PaymentItems)
        {
            var paymentItem = new PaymentItem(item.MovieName, item.MovieId, item.Price, item.Quantity);
            payment.AddPaymentItem(paymentItem);
        }
        // ensure aggregate total is up to date
        payment.RecalculateTotal(command.Currency);
        return payment;
    }
}