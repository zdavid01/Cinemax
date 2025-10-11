using Grpc.Core;
using Payment.API.Protos;

namespace Basket.API.GrpcServices;

public class PaymentGrpcClient
{
    private readonly PaymentService.PaymentServiceClient _client;
    private readonly ILogger<PaymentGrpcClient> _logger;

    public PaymentGrpcClient(PaymentService.PaymentServiceClient client, ILogger<PaymentGrpcClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CheckoutResponse> InitiateCheckout(CheckoutRequest request)
    {
        try
        {
            _logger.LogInformation($"Initiating checkout via gRPC for user {request.BuyerUsername}");
            
            var response = await _client.InitiateCheckoutAsync(request);
            
            if (response.Success)
            {
                _logger.LogInformation($"Checkout successful. Payment ID: {response.PaymentId}");
            }
            else
            {
                _logger.LogWarning($"Checkout failed: {response.Message}");
            }
            
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"gRPC call failed with status {ex.Status.StatusCode}: {ex.Status.Detail}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during gRPC checkout");
            throw;
        }
    }
}

