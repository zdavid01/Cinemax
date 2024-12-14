namespace Payment.Application.Contracts.Persistence;

public interface IOrderRepository : IAsyncRepository<Domain.Aggregates.Payment>
{
    Task<IReadOnlyCollection<Domain.Aggregates.Payment>> GetPaymentByUsername(string username);
}