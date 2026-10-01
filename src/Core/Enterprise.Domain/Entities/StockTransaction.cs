using Enterprise.Domain.Common;
using Enterprise.Domain.Enums;
using Enterprise.Domain.Exceptions;

namespace Enterprise.Domain.Entities;

/// <summary>
/// Entity representing an inventory transaction log for a product.
/// </summary>
public class StockTransaction : BaseAuditableEntity<Guid>
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public StockTransactionType TransactionType { get; private set; }
    public string Reason { get; private set; } = default!;
    public DateTime TransactionDateUtc { get; private set; }

    private StockTransaction() : base()
    {
    }

    private StockTransaction(Guid id, Guid productId, int quantity, StockTransactionType transactionType, string reason)
        : base(id)
    {
        ProductId = productId;
        Quantity = quantity;
        TransactionType = transactionType;
        Reason = reason;
        TransactionDateUtc = DateTime.UtcNow;
    }

    public static StockTransaction Create(Guid productId, int quantity, StockTransactionType transactionType, string reason)
    {
        if (productId == Guid.Empty)
            throw new DomainValidationException("ProductId cannot be empty.");

        if (quantity <= 0)
            throw new DomainValidationException("Transaction quantity must be greater than zero.");

        return new StockTransaction(Guid.Empty, productId, quantity, transactionType, reason ?? string.Empty);
    }
}
