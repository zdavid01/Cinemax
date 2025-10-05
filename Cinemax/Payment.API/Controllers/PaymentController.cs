using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.Payments.Commands.CreatePayment;
using Payment.Application.Features.Payments.Queries.GetListOfPaymentsQuery;
using Payment.Application.Features.Payments.Queries.ViewModels;

namespace Payment.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("get-payments/{username}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentViewModel>>> GetPayments(string username)
    {
        var query = new GetListOfPaymentsQuery(username);
        var payments = await _mediator.Send(query);
        return Ok(payments);
    }

    [HttpPost("create-payment")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CreatePayment([FromBody] CreatePaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("test-payment/{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> TestPayment(int id)
    {
        // This is a test endpoint to debug the Payment entity loading
        var payment = await _mediator.Send(new GetListOfPaymentsQuery("ddd"));
        var testPayment = payment.FirstOrDefault(p => p.Id == id);
        
        if (testPayment == null)
        {
            return NotFound($"Payment with ID {id} not found");
        }
        
        return Ok(new {
            id = testPayment.Id,
            paymentDate = testPayment.PaymentDate,
            createdDate = testPayment.CreatedDate,
            createdBy = testPayment.CreatedBy,
            amount = testPayment.Amount,
            currency = testPayment.Currency,
            buyerUsername = testPayment.BuyerUsername,
            paymentItems = testPayment.PaymentItems
        });
    }

    [HttpGet("debug-payment-items")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> DebugPaymentItems()
    {
        // Direct database query to debug PaymentItems
        var payments = await _mediator.Send(new GetListOfPaymentsQuery("ddd"));
        
        return Ok(new {
            totalPayments = payments.Count,
            payments = payments.Select(p => new {
                id = p.Id,
                itemCount = p.PaymentItems?.Count ?? 0,
                items = p.PaymentItems?.Select(i => new {
                    id = i.Id,
                    movieName = i.MovieName,
                    movieId = i.MovieId,
                    price = i.Price,
                    quantity = i.Quantity
                }) ?? new object[0]
            })
        });
    }
}