using Enterprise.Domain.Common;
using Enterprise.Domain.Exceptions;
using Enterprise.Domain.ValueObjects;

namespace Enterprise.Domain.Entities;

public class OrderItem : BaseEntity<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public Money UnitPrice { get; private set; } = default!;
    public int Quantity { get; private set; }
    public Money TotalPrice => Money.Create(UnitPrice.Amount * Quantity, UnitPrice.Currency);

    private OrderItem() : base() { }

    private OrderItem(Guid id, Guid productId, string productName, Money unitPrice, int quantity) : base(id)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public static OrderItem Create(Guid productId, string productName, Money unitPrice, int quantity)
    {
        if (productId == Guid.Empty)
            throw new DomainValidationException("Invalid product ID.");
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainValidationException("Product name cannot be empty.");
        ArgumentNullException.ThrowIfNull(unitPrice);
        if (quantity <= 0)
            throw new DomainValidationException("Quantity must be greater than zero.");

        return new OrderItem(Guid.NewGuid(), productId, productName, unitPrice, quantity);
    }
}
