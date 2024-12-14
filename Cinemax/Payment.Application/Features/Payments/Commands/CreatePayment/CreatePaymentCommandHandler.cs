using MediatR;
using Payment.Application.Contracts.Persistence;
using Payment.Application.Factories;

namespace Payment.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommandHandler
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentFactory _factory;

    public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IPaymentFactory factory)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public async Task<int> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = _factory.Create(request);
        var newPayment = await _paymentRepository.AddAsync(payment);
        
        return newPayment.Id;
    }
    
}