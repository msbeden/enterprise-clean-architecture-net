using Enterprise.Domain.Common;
using Enterprise.Domain.Enums;

namespace Enterprise.Domain.Events;

/// <summary>
/// Event triggered when product stock is increased or decreased.
/// </summary>
public record StockUpdatedEvent(
    Guid ProductId,
    int NewStockQuantity,
    int QuantityChanged,
    StockTransactionType TransactionType
) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
