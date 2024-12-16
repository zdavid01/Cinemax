using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Infrastructure.Persistence;

public class PaymentContextSeed
{
    public static async Task SeedAsync(PaymentContext paymentContext, ILogger<PaymentContext> logger)
    {
        if (!paymentContext.Payments.Any())
        {
            paymentContext.Payments.AddRange(GetPreconfiguredPayments());
            await paymentContext.SaveChangesAsync();
            
            logger.LogInformation($"Seeding database with context {nameof(PaymentContext)}.");
        }
    }

    private static IEnumerable<Domain.Aggregates.Payment> GetPreconfiguredPayments()
    {
        var payment1 = new Domain.Aggregates.Payment("B001", "dz", "USD");

        payment1.AddPaymentItem(new PaymentItem("Sekula 2", "M0014", 250, 1));
        payment1.AddPaymentItem(new PaymentItem("Sojic", "M0024", 150, 2));
        
        return new List<Domain.Aggregates.Payment> { payment1 };
    }
}