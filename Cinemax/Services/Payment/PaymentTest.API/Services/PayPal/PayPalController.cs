using Microsoft.AspNetCore.Mvc;
using PaymentTest.API.Services.Email.Contracts;
using PaymentTest.API.Services.Email.Persistance;
using PaymentTest.API.Services.Email.Services;

namespace Paypal;

[Route("api/paypal")]
[ApiController]
public class PayPalController : ControllerBase
{
    private readonly PayPalService _payPalService;
    private readonly IMailService _emailService;
    
    public PayPalController(IMailService mailService)
    {
        _payPalService = new PayPalService();
        _emailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    // Endpoint to initiate the payment process
    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest paymentRequest)
    {
        try
        {
            var payment = await _payPalService.CreatePayment(paymentRequest.Amount, paymentRequest.Currency);

            SendEmailRequest emailRequest = new SendEmailRequest(From: "dzivkovicd1@gmail.com", To:"dzivkovicd1@gmail.com", Subject:"CreatedPaypal", Body:"Najveci");
            
            await _emailService.SendEmailAsync(emailRequest);

            return Ok(payment);
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