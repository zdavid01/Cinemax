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
}