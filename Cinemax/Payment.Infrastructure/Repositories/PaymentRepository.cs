using Microsoft.EntityFrameworkCore;
using Payment.Application.Contracts.Persistence;

namespace Payment.Infrastructure.Repositories;

public class PaymentRepository : RepositoryBase<Domain.Aggregates.Payment>, IPaymentRepository
{
    public async Task<IReadOnlyCollection<Domain.Aggregates.Payment>> GetPaymentsByUsername(string username)
    {
        return await _dbContext.Payments
            .Where(p => p.BuyerUsername == username)
            .Include(p => p.PaymentItems)
            .ToListAsync();
    }
}