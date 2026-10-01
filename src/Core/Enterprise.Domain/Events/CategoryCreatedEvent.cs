using Enterprise.Domain.Common;

namespace Enterprise.Domain.Events;

public record CategoryCreatedEvent(
    Guid CategoryId,
    string Name,
    int DisplayOrder
) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
