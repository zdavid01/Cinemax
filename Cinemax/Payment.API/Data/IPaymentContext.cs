using Npgsql;

namespace Payment.API.Data;

public interface IPaymentContext
{
    NpgsqlConnection GetConnection();
}