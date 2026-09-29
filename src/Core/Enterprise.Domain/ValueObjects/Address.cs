using Enterprise.Domain.Exceptions;

namespace Enterprise.Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string Country { get; private set; }

    private Address()
    {
        Street = string.Empty;
        City = string.Empty;
        State = string.Empty;
        ZipCode = string.Empty;
        Country = string.Empty;
    }

    public Address(string street, string city, string state, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainValidationException("Street cannot be empty.");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainValidationException("City cannot be empty.");
        if (string.IsNullOrWhiteSpace(country))
            throw new DomainValidationException("Country cannot be empty.");

        Street = street;
        City = city;
        State = state ?? string.Empty;
        ZipCode = zipCode ?? string.Empty;
        Country = country;
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }
}
