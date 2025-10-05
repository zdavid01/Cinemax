using Microsoft.AspNetCore.Mvc;
using Payment.Infrastructure.PayPal;
using RestSharp;
using Payment.Application.Contracts.Infrastructure;
using Payment.Application.Models;

namespace Payment.API.Controllers;

[Route("api/paypal")]
[ApiController]
public class PayPalController : ControllerBase
{
    private readonly PayPalService? _payPalService;
    private readonly ILogger<PayPalController> _logger;
    private readonly IEmailService _emailService;

    public PayPalController(ILogger<PayPalController> logger, IEmailService emailService, PayPalService payPalService)
    {
        _payPalService = payPalService ?? throw new ArgumentNullException(nameof(payPalService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }
    
    // Endpoint to initiate the payment process
    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest paymentRequest)
    {
        try
        {
            var (paymentId, approvalUrl) = await _payPalService.CreatePayment(paymentRequest.Amount, paymentRequest.Currency);
            
            // Return the expected JSON structure
            var response = new
            {
                id = paymentId,
                state = "created",
                links = new[]
                {
                    new
                    {
                        href = approvalUrl,
                        rel = "approval_url",
                        method = "REDIRECT"
                    }
                }
            };
            
            return Ok(response);
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
            if (state == "approved")
            {
                await SendSuccessEmail(executeRequest.PaymentId);
                return Ok(new { state, message = "Payment successfully completed and email sent." });
            }
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

            // If the payment was successful, return an HTML page that closes the popup and notifies the parent
            if (paymentState == "approved")
            {
                await SendSuccessEmail(paymentId);
                
                var html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Payment Successful</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; background-color: #f0f8f0; }}
                        .success {{ color: #4caf50; font-size: 24px; margin-bottom: 20px; }}
                        .message {{ color: #333; font-size: 16px; margin-bottom: 30px; }}
                        .close-btn {{ background-color: #4caf50; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; }}
                    </style>
                </head>
                <body>
                    <div class='success'>✅ Payment Successful!</div>
                    <div class='message'>Your payment has been processed and a confirmation email has been sent.</div>
                    <button class='close-btn' onclick='closeAndNotify()'>Close Window</button>
                    
                    <script>
                        function closeAndNotify() {{
                            // Notify the parent window about the successful payment
                            if (window.opener) {{
                                window.opener.postMessage({{
                                    type: 'PAYPAL_PAYMENT_SUCCESS',
                                    paymentId: '{paymentId}',
                                    payerId: '{payerId}',
                                    state: '{paymentState}'
                                }}, '*');
                            }}
                            window.close();
                        }}
                        
                        // Auto-close after 3 seconds
                        setTimeout(closeAndNotify, 3000);
                    </script>
                </body>
                </html>";
                
                return Content(html, "text/html");
            }
            else
            {
                var html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Payment Failed</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; background-color: #ffeaea; }}
                        .error {{ color: #f44336; font-size: 24px; margin-bottom: 20px; }}
                        .message {{ color: #333; font-size: 16px; margin-bottom: 30px; }}
                        .close-btn {{ background-color: #f44336; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; }}
                    </style>
                </head>
                <body>
                    <div class='error'>❌ Payment Failed</div>
                    <div class='message'>Payment was not approved. Please try again.</div>
                    <button class='close-btn' onclick='closeAndNotify()'>Close Window</button>
                    
                    <script>
                        function closeAndNotify() {{
                            // Notify the parent window about the failed payment
                            if (window.opener) {{
                                window.opener.postMessage({{
                                    type: 'PAYPAL_PAYMENT_FAILED',
                                    paymentId: '{paymentId}',
                                    payerId: '{payerId}',
                                    state: '{paymentState}'
                                }}, '*');
                            }}
                            window.close();
                        }}
                        
                        // Auto-close after 3 seconds
                        setTimeout(closeAndNotify, 3000);
                    </script>
                </body>
                </html>";
                
                return Content(html, "text/html");
            }
        }
        catch (Exception ex)
        {
            var html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Payment Error</title>
                <style>
                    body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; background-color: #ffeaea; }}
                    .error {{ color: #f44336; font-size: 24px; margin-bottom: 20px; }}
                    .message {{ color: #333; font-size: 16px; margin-bottom: 30px; }}
                    .close-btn {{ background-color: #f44336; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; }}
                </style>
            </head>
            <body>
                <div class='error'>❌ Payment Error</div>
                <div class='message'>An error occurred: {ex.Message}</div>
                <button class='close-btn' onclick='window.close()'>Close Window</button>
            </body>
            </html>";
            
            return Content(html, "text/html");
        }
    }

    // Test endpoint for development - simulates successful payment execution
    [HttpPost("test-execute-payment")]
    public async Task<IActionResult> TestExecutePayment([FromBody] ExecutePaymentRequest executeRequest)
    {
        try
        {
            // For testing purposes, simulate a successful payment
            var email = new Email() 
            { 
                To = "destin.schroeder4@ethereal.email", 
                Body = $"Test payment successfully completed! Payment ID: {executeRequest.PaymentId}, Payer ID: {executeRequest.PayerId}", 
                Subject = "Test Payment Confirmation" 
            };
            await _emailService.SendEmail(email);
            
            return Ok(new { 
                state = "approved", 
                message = "Test payment successfully completed and email sent.",
                paymentId = executeRequest.PaymentId,
                payerId = executeRequest.PayerId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test payment execution");
            return BadRequest(new { error = $"Error in test payment execution: {ex.Message}" });
        }
    }

    private async Task SendSuccessEmail(string paymentId)
    {
        try
        {
            // Note: No buyer email provided in current flow; send to configured inbox for confirmation
            var email = new Email
            {
                To = "destin.schroeder4@ethereal.email",
                Subject = $"Payment {paymentId} completed",
                Body = $"Your PayPal payment with ID {paymentId} has been approved."
            };
            await _emailService.SendEmail(email);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send payment confirmation email for {PaymentId}", paymentId);
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