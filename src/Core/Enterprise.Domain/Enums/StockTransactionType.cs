namespace Enterprise.Domain.Enums;

/// <summary>
/// Type of inventory stock transaction.
/// </summary>
public enum StockTransactionType
{
    Inbound = 1,
    Outbound = 2,
    Adjustment = 3
}
