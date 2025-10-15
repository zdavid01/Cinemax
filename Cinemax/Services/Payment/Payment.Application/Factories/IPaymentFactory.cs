using Payment.Application.Features.Payments.Commands.CreatePayment;

namespace Payment.Application.Factories;

public interface IPaymentFactory
{
    Domain.Aggregates.Payment Create(CreatePaymentCommand command);
}