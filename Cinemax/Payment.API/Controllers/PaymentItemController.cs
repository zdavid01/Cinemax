using Microsoft.AspNetCore.Mvc;
using Payment.API.DTOs;
using Payment.API.Repositories;

namespace Payment.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaymentItemController : ControllerBase
{
    private readonly IPaymentRepository _repository;

    public PaymentItemController(IPaymentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet("paymentId")]
    [ProducesResponseType(typeof(PaymentItemDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentItemDTO>> GetPaymentItemAsync(int paymentId)
    {
        var paymentItem = await _repository.GetPaymentItem(paymentId);
        
        Console.WriteLine(paymentItem);
        if (paymentItem is null)
        {
            return NotFound();
        }
        
        return Ok(paymentItem);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PaymentItemDTO), StatusCodes.Status201Created)]
    public async Task<ActionResult<PaymentItemDTO>> CreatePaymentItem([FromBody] CreatePaymentItemDTO paymentItem)
    {
        paymentItem.CreatedAt = DateTime.Now;
        return Ok(await _repository.CreatePaymentItem(paymentItem));
    }
    
    [HttpDelete("id", Name = "DeletePaymentItem")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> DeletePaymentItem(int paymentItemId)
    {
    
        return Ok(await _repository.DeletePaymentItem(paymentItemId));
    }
    
    [HttpGet("movieId")]
    [ProducesResponseType(typeof(IEnumerable<PaymentItemDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PaymentItemDTO>>> GetPaymentItemsForMovie(string movieId)
    {
        var paymentItems = await _repository.GetPaymentItemsForMovie(movieId);
    
        if (!paymentItems.Any())
        {
            return NotFound();
        }
        
        return Ok(paymentItems);
    }
    
}