using Enterprise.Domain.Entities;
using Enterprise.Domain.Enums;
using Enterprise.Domain.Events;
using Enterprise.Domain.Exceptions;
using Enterprise.Domain.ValueObjects;
using Xunit;

namespace Enterprise.Domain.UnitTests.Entities;

public class OrderTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCalculateTotalAmountAndRaiseEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var address = new Address("Main St 123", "Istanbul", "TR", "34000", "Turkey");
        var price = Money.Create(100m, "TRY");
        var item1 = OrderItem.Create(Guid.NewGuid(), "Item 1", price, 2);
        var item2 = OrderItem.Create(Guid.NewGuid(), "Item 2", price, 1);

        // Act
        var order = Order.Create(userId, address, new List<OrderItem> { item1, item2 });

        // Assert
        Assert.NotNull(order);
        Assert.Equal(userId, order.UserId);
        Assert.Equal(300m, order.TotalAmount.Amount);
        Assert.Equal("TRY", order.TotalAmount.Currency);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(2, order.Items.Count);
        Assert.Single(order.DomainEvents);
        Assert.IsType<OrderCreatedEvent>(order.DomainEvents.First());
    }

    [Fact]
    public void Cancel_PendingOrder_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var address = new Address("Main St 123", "Istanbul", "TR", "34000", "Turkey");
        var item = OrderItem.Create(Guid.NewGuid(), "Item 1", Money.Create(50m, "TRY"), 1);
        var order = Order.Create(Guid.NewGuid(), address, new List<OrderItem> { item });

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_CompletedOrder_ShouldThrowDomainValidationException()
    {
        // Arrange
        var address = new Address("Main St 123", "Istanbul", "TR", "34000", "Turkey");
        var item = OrderItem.Create(Guid.NewGuid(), "Item 1", Money.Create(50m, "TRY"), 1);
        var order = Order.Create(Guid.NewGuid(), address, new List<OrderItem> { item });
        order.UpdateStatus(OrderStatus.Completed);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => order.Cancel());
    }
}
