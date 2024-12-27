using Npgsql;

namespace PaymentTest.API.Data;

public class PaymentContext : IPaymentContext
{
    private readonly IConfiguration _configuration;

    public PaymentContext(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
    }
}