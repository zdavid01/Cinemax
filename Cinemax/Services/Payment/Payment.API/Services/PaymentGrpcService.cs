using Grpc.Core;
using MediatR;
using Payment.API.Protos;
using Payment.Application.Features.Payments.Commands.CreatePayment;
using Payment.Application.Features.Payments.Commands.DTOs;

namespace Payment.API.Services;

public class PaymentGrpcService : PaymentService.PaymentServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentGrpcService> _logger;

    public PaymentGrpcService(IMediator mediator, ILogger<PaymentGrpcService> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<CheckoutResponse> InitiateCheckout(CheckoutRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation($"Starting checkout for user {request.BuyerUsername} with {request.Items.Count} items");

            // Map gRPC request to MediatR command
            var createPaymentCommand = new CreatePaymentCommand
            {
                BuyerId = request.BuyerId,
                BuyerUsername = request.BuyerUsername,
                Amount = (decimal)request.TotalPrice,
                Currency = string.IsNullOrEmpty(request.Currency) ? "USD" : request.Currency,
                PaymentItems = request.Items.Select(item => new PaymentItemDTO
                {
                    MovieId = item.MovieId,
                    MovieName = item.MovieName,
                    Price = (decimal)item.Price,
                    Quantity = item.Quantity
                }).ToList()
            };

            // Execute the command
            var paymentId = await _mediator.Send(createPaymentCommand);

            _logger.LogInformation($"Payment {paymentId} created successfully for user {request.BuyerUsername}");

            // Return success response
            return new CheckoutResponse
            {
                Success = true,
                Message = "Payment initiated successfully",
                PaymentId = paymentId,
                PaymentDate = DateTime.UtcNow.ToString("o")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error initiating checkout for user {request.BuyerUsername}");
            
            return new CheckoutResponse
            {
                Success = false,
                Message = $"Failed to initiate payment: {ex.Message}",
                PaymentId = 0,
                PaymentDate = DateTime.UtcNow.ToString("o")
            };
        }
    }
}

