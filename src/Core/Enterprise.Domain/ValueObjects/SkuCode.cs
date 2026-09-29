using Enterprise.Domain.Exceptions;

namespace Enterprise.Domain.ValueObjects;

/// <summary>
/// Value Object representing Stock Keeping Unit (SKU) code.
/// </summary>
public class SkuCode : ValueObject
{
    public string Value { get; }

    private SkuCode()
    {
        Value = default!;
    }

    private SkuCode(string value)
    {
        Value = value;
    }

    public static SkuCode Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainValidationException("SKU code cannot be empty.");

        var trimmed = code.Trim().ToUpperInvariant();
        if (trimmed.Length < 3 || trimmed.Length > 30)
            throw new DomainValidationException("SKU code must be between 3 and 30 characters.");

        return new SkuCode(trimmed);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
