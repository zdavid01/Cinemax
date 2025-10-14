namespace PrivateSession.Services;

public interface IPaymentService
{
    Task<IEnumerable<string>> GetPurchasedMovieNamesAsync(string username);
}

