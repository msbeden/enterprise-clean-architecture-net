using Enterprise.Domain.Common;
using Enterprise.Domain.Enums;

namespace Enterprise.Domain.Events;

public record OrderStatusChangedEvent(
    Guid OrderId,
    OrderStatus OldStatus,
    OrderStatus NewStatus
) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
