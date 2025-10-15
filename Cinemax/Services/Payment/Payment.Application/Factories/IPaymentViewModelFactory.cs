using Payment.Application.Features.Payments.Queries.ViewModels;

namespace Payment.Application.Factories;

public interface IPaymentViewModelFactory
{
    PaymentViewModel CreateViewModel (Domain.Aggregates.Payment payment);
}