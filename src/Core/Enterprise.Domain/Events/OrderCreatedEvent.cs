using Enterprise.Domain.Common;

namespace Enterprise.Domain.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid UserId,
    decimal TotalAmount,
    string Currency,
    int ItemCount
) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
