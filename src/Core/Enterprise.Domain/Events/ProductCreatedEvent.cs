using Enterprise.Domain.Common;

namespace Enterprise.Domain.Events;

/// <summary>
/// Event triggered when a new product is created in the domain.
/// </summary>
public record ProductCreatedEvent(
    Guid ProductId,
    string Name,
    string Sku,
    decimal PriceAmount,
    string Currency,
    int StockQuantity
) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
