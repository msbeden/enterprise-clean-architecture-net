using Enterprise.Domain.Entities;
using Enterprise.Domain.Events;
using Enterprise.Domain.Exceptions;
using Enterprise.Domain.ValueObjects;
using Xunit;

namespace Enterprise.Domain.UnitTests.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldInitializeProductAndRaiseDomainEvent()
    {
        // Arrange
        var sku = SkuCode.Create("PRD-001");
        var price = Money.Create(150m, "USD");

        // Act
        var product = Product.Create("Laptop", "High end laptop", sku, price, 10);

        // Assert
        Assert.NotNull(product);
        Assert.Equal("Laptop", product.Name);
        Assert.Equal(10, product.StockQuantity);
        Assert.True(product.IsActive);
        Assert.Single(product.StockTransactions);
        Assert.Single(product.DomainEvents);
        Assert.IsType<ProductCreatedEvent>(product.DomainEvents.First());
    }

    [Fact]
    public void IncreaseStock_ShouldIncreaseStockAndLogTransaction()
    {
        // Arrange
        var product = Product.Create("Mouse", "Wireless Mouse", SkuCode.Create("MSE-001"), Money.Create(25m, "USD"), 5);
        product.ClearDomainEvents();

        // Act
        product.IncreaseStock(15, "New shipment arrived");

        // Assert
        Assert.Equal(20, product.StockQuantity);
        Assert.Equal(2, product.StockTransactions.Count);
        Assert.Single(product.DomainEvents);
        Assert.IsType<StockUpdatedEvent>(product.DomainEvents.First());
    }

    [Fact]
    public void DecreaseStock_WithExceedingQuantity_ShouldThrowDomainValidationException()
    {
        // Arrange
        var product = Product.Create("Keyboard", "Mechanical Keyboard", SkuCode.Create("KBD-001"), Money.Create(80m, "USD"), 5);

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => product.DecreaseStock(10, "Order #123"));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var product = Product.Create("Monitor", "4K Monitor", SkuCode.Create("MNT-001"), Money.Create(300m, "USD"), 2);

        // Act
        product.Deactivate();

        // Assert
        Assert.False(product.IsActive);
    }
}
