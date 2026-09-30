using Enterprise.Domain.Common;
using Enterprise.Domain.Enums;

namespace Enterprise.Domain.Events;

public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    UserRole Role
) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
