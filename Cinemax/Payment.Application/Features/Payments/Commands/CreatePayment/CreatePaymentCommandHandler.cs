using MediatR;
using Payment.Application.Contracts.Persistence;
using Payment.Application.Factories;

namespace Payment.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, int>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentFactory _factory;

    private readonly ILogger<CreatePaymentCommandHandler> _logger;
    public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IPaymentFactory factory, ILogger<CreatePaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = _factory.Create(request);
        var newPayment = await _paymentRepository.AddAsync(payment);
        
        _logger.LogInformation($"Payment with Id: {payment.Id} has been created");
        
        return newPayment.Id;
    }
    
}