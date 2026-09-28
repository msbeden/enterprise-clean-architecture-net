namespace Enterprise.Domain.Common;

/// <summary>
/// Marker interface for Domain Events in the system.
/// Domain layer stays pure POCO without external dependencies.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
