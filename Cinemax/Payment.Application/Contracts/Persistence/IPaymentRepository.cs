namespace Payment.Application.Contracts.Persistence;

public interface IPaymentRepository : IAsyncRepository<Domain.Aggregates.Payment>
{
    Task<IReadOnlyCollection<Domain.Aggregates.Payment>> GetPaymentsByUsername(string username);
}