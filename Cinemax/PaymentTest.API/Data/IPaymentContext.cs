using Npgsql;

namespace PaymentTest.API.Data;

public interface IPaymentContext
{
    NpgsqlConnection GetConnection();
}