using System;
using System.Threading.Tasks;
using Com.CloudRail.SI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payment.Infrastructure.PayPal;
using RestSharp;

namespace Payment.API.Controllers;

[Route("api/paypal")]
[ApiController]
public class PayPalController : ControllerBase
{
    private readonly PayPalService? _payPalService;
    private readonly ILogger<PayPalController> _logger;

    public PayPalController( ILogger<PayPalController> logger)
    {
        _payPalService = new PayPalService();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    // Endpoint to initiate the payment process
    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest paymentRequest)
    {
        try
        {
            var payment = await _payPalService.CreatePayment(paymentRequest.Amount, paymentRequest.Currency);
            return Ok( payment );
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Endpoint to execute the payment after approval
    [HttpPost("execute-payment")]
    public async Task<IActionResult> ExecutePayment([FromBody] ExecutePaymentRequest executeRequest)
    {
        try
        {
            var state = await _payPalService.ExecutePayment(executeRequest.PaymentId, executeRequest.PayerId);
            return Ok(new { state });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpGet("return")]
    public async Task<IActionResult> ReturnFromPayPal(string paymentId, string payerId)
    {
        try
        {
            // Execute the payment after the user approves it
            var paymentState = await _payPalService.ExecutePayment(paymentId, payerId);

            // If the payment was successful, return a success message
            if (paymentState == "approved")
            {
                return Ok("Payment successfully completed.");
            }
            else
            {
                return BadRequest("Payment not approved.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

public class ExecutePaymentRequest
{
    public string PaymentId { get; set; }
    public string PayerId { get; set; }
}