using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentTest.API.Data.DTOs.Payment;
using PaymentTest.API.Data.Payment;
using PaymentTest.API.Entities;

namespace PaymentTest.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly PaymentContext _context;

    public PaymentController(PaymentContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaymentDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
    {
        var payments = await _context.Payments.Include(p => true).ToListAsync();

        if (payments is null)
        {
            return NotFound();
        }
        
        return Ok(payments);
    }
}