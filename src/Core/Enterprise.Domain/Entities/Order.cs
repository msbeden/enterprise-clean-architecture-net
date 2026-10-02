using Enterprise.Domain.Common;
using Enterprise.Domain.Enums;
using Enterprise.Domain.Events;
using Enterprise.Domain.Exceptions;
using Enterprise.Domain.ValueObjects;

namespace Enterprise.Domain.Entities;

public class Order : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public Address ShippingAddress { get; private set; } = default!;
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; } = default!;

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() : base() { }

    private Order(Guid id, Guid userId, Address shippingAddress) : base(id)
    {
        UserId = userId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        TotalAmount = Money.Create(0, "TRY");
    }

    public static Order Create(Guid userId, Address shippingAddress, List<OrderItem> items)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("Invalid user ID.");
        ArgumentNullException.ThrowIfNull(shippingAddress);
        if (items == null || items.Count == 0)
            throw new DomainValidationException("Order must contain at least one item.");

        var order = new Order(Guid.NewGuid(), userId, shippingAddress);

        string currency = items[0].UnitPrice.Currency;
        decimal total = 0;

        foreach (var item in items)
        {
            if (item.UnitPrice.Currency != currency)
                throw new DomainValidationException("All items in an order must use the same currency.");

            order._items.Add(item);
            total += item.TotalPrice.Amount;
        }

        order.TotalAmount = Money.Create(total, currency);

        order.AddDomainEvent(new OrderCreatedEvent(
            order.Id,
            order.UserId,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            order._items.Count
        ));

        return order;
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        if (Status == newStatus) return;

        var oldStatus = Status;
        Status = newStatus;

        AddDomainEvent(new OrderStatusChangedEvent(Id, oldStatus, newStatus));
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Shipped)
            throw new DomainValidationException("Shipped or completed orders cannot be cancelled.");

        UpdateStatus(OrderStatus.Cancelled);
    }
}
