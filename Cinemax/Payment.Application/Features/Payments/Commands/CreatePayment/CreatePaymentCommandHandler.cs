using MediatR;
using Payment.Application.Contracts.Infrastructure;
using Payment.Application.Contracts.Persistence;
using Payment.Application.Factories;
using Payment.Application.Models;

namespace Payment.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, int>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentFactory _factory;
    private readonly IEmailService _emailService;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;
    public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IPaymentFactory factory, ILogger<CreatePaymentCommandHandler> logger, IEmailService emailService)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task<int> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = _factory.Create(request);
        var newPayment = await _paymentRepository.AddAsync(payment);
        
        _logger.LogInformation($"Payment with Id: {payment.Id} has been created");
        
        return newPayment.Id;
    }

    public async Task SendMail(Domain.Aggregates.Payment newPayment)
    {
        var email = new Email
        {
            //TODO user email
            // To = newPayment.Email,
            Subject = $"Order {newPayment.Id} is successfully created",
            Body = "You have completed a new payment!",
        };

        try
        {
            await _emailService.SendEmail(email);
            _logger.LogInformation($"Sending email for payment {newPayment.Id} was successful");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Failed to send email for payment {newPayment.Id} due to error: {e.Message}");
        }
    }
    
}