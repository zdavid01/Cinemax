using Payment.Domain.Common;

namespace Payment.Domain.ValueObjects;

public class Money : ValueObject
{
    public int Amount;
    public string Currency;

    public Money(int amount, string currency)
    {
        Amount = amount;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}