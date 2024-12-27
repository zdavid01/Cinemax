using Npgsql;

namespace PaymentTest.API.Data;

public interface IPaymentItemContext
{
    NpgsqlConnection GetConnection();
}