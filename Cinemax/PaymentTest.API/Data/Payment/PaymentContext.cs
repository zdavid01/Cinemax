using Microsoft.EntityFrameworkCore;
using PaymentTest.API.Entities;

namespace PaymentTest.API.Data.Payment;

public class PaymentContext : DbContext , IPaymentContext
{
    public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
    {
    }
    
    public DbSet<Entities.Payment> Payments { get; set; }
}