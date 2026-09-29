namespace Enterprise.Domain.Exceptions;

/// <summary>
/// Exception thrown when a domain rule or invariant is violated.
/// </summary>
public class DomainValidationException : DomainException
{
    public DomainValidationException(string message) : base(message)
    {
    }
}
