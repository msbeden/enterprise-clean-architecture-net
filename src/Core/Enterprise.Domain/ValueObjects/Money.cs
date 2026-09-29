using Enterprise.Domain.Exceptions;

namespace Enterprise.Domain.ValueObjects;

/// <summary>
/// Value Object representing monetary value with currency.
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = default!;

    private Money()
    {
        Amount = 0;
        Currency = "USD";
    }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Create(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new DomainValidationException("Monetary amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
            throw new DomainValidationException("Currency code must be a 3-letter ISO code.");

        return new Money(amount, currency.Trim());
    }

    public static Money Zero(string currency = "USD") => Create(0, currency);

    public Money Add(Money money)
    {
        EnsureSameCurrency(money);
        return new Money(Amount + money.Amount, Currency);
    }

    public Money Subtract(Money money)
    {
        EnsureSameCurrency(money);
        if (Amount - money.Amount < 0)
            throw new DomainValidationException("Resulting monetary amount cannot be negative.");

        return new Money(Amount - money.Amount, Currency);
    }

    private void EnsureSameCurrency(Money money)
    {
        if (Currency != money.Currency)
            throw new DomainValidationException($"Cannot perform arithmetic operations on different currencies: {Currency} and {money.Currency}.");
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
