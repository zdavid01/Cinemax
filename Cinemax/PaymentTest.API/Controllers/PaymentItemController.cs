using Microsoft.AspNetCore.Mvc;
using PaymentTest.API.Data.DTOs;
using PaymentTest.API.Repositories;

namespace PaymentTest.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaymentItemController : ControllerBase
{
    
    private readonly IPaymentItemRepository _repository;
    
    public PaymentItemController(IPaymentItemRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(PaymentItemDTO), 200)]
    public async Task<ActionResult<IEnumerable<PaymentItemDTO>>> GetPaymentItems()
    {
        var paymentItems = await _repository.GetPaymentItems();
        
        return Ok(paymentItems);
    }
    
    [HttpGet("{movieId}", Name = "GetPaymentItemByMovieId")]
    [ProducesResponseType(typeof(PaymentItemDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentItemDTO>> GetPaymentItemByMovieId(string movieId)
    {
        var paymentItem = await _repository.GetPaymentItemByMovieId(movieId);
        if (paymentItem == null)
        {
            return NotFound();
        }
        
        return Ok(paymentItem);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(PaymentItemDTO), StatusCodes.Status201Created)]
    public async Task<ActionResult<PaymentItemDTO>> CreatePaymentItem([FromBody] CreatePaymentItemDTO paymentItemDTO)
    {
        await _repository.CreatePaymentItem(paymentItemDTO);
    
        var paymentItem = await _repository.GetPaymentItemByMovieId(paymentItemDTO.MovieId);
    
        return CreatedAtRoute("GetPaymentItemByMovieId", new { movieId = paymentItem.MovieId }, paymentItem);
    }
    
    [HttpPut]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> UpdateDiscount([FromBody] UpdatePaymentItemDTO paymentItem)
    {
        return Ok(await _repository.UpdatePaymentItem(paymentItem));
    }

    [HttpDelete("{id}", Name = "DeletePaymentItem")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> DeleteDiscount(int id)
    {
        return Ok(await _repository.DeletePaymentItem(id));
    }
    
    [HttpGet("[action]/{movieId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentItemDTO>>> GetPaymentItemsByMovieId(string movieId)
    {
        var paymentItems = await _repository.GetPaymentItemsByMovieId(movieId);
        
        return Ok(paymentItems);
    }
    
    [HttpGet("[action]/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentItemDTO>>> GetPaymentItemsByUserId(string userId)
    {
        var paymentItems = await _repository.GetPaymentItemsByUserId(userId);
        
        return Ok(paymentItems);
    }
}