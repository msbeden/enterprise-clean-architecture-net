using Enterprise.Domain.Common;
using Enterprise.Domain.Enums;
using Enterprise.Domain.Events;
using Enterprise.Domain.Exceptions;
using Enterprise.Domain.ValueObjects;

namespace Enterprise.Domain.Entities;

/// <summary>
/// Aggregate Root representing a product in the catalog.
/// </summary>
public class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public SkuCode Sku { get; private set; } = default!;
    public Money Price { get; private set; } = default!;
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private readonly List<StockTransaction> _stockTransactions = new();
    public IReadOnlyCollection<StockTransaction> StockTransactions => _stockTransactions.AsReadOnly();

    private Product() : base()
    {
    }

    private Product(Guid id, string name, string description, SkuCode sku, Money price, int initialStock)
        : base(id)
    {
        Name = name;
        Description = description;
        Sku = sku;
        Price = price;
        StockQuantity = initialStock;
        IsActive = true;
    }

    public static Product Create(string name, string description, SkuCode sku, Money price, int initialStock = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Product name cannot be empty.");

        if (initialStock < 0)
            throw new DomainValidationException("Initial stock quantity cannot be negative.");

        var product = new Product(Guid.NewGuid(), name, description ?? string.Empty, sku, price, initialStock);

        if (initialStock > 0)
        {
            product._stockTransactions.Add(StockTransaction.Create(
                product.Id,
                initialStock,
                StockTransactionType.Inbound,
                "Initial stock setup"
            ));
        }

        product.AddDomainEvent(new ProductCreatedEvent(
            product.Id,
            product.Name,
            product.Sku.Value,
            product.Price.Amount,
            product.Price.Currency,
            product.StockQuantity
        ));

        return product;
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Product name cannot be empty.");

        Name = name;
        Description = description ?? string.Empty;
    }

    public void UpdatePrice(Money newPrice)
    {
        ArgumentNullException.ThrowIfNull(newPrice);
        Price = newPrice;
    }

    public void IncreaseStock(int quantity, string reason)
    {
        if (quantity <= 0)
            throw new DomainValidationException("Quantity to increase must be greater than zero.");

        StockQuantity += quantity;
        var transaction = StockTransaction.Create(Id, quantity, StockTransactionType.Inbound, reason);
        _stockTransactions.Add(transaction);

        AddDomainEvent(new StockUpdatedEvent(Id, StockQuantity, quantity, StockTransactionType.Inbound));
    }

    public void DecreaseStock(int quantity, string reason)
    {
        if (quantity <= 0)
            throw new DomainValidationException("Quantity to decrease must be greater than zero.");

        if (StockQuantity < quantity)
            throw new DomainValidationException($"Insufficient stock. Available: {StockQuantity}, requested: {quantity}.");

        StockQuantity -= quantity;
        var transaction = StockTransaction.Create(Id, quantity, StockTransactionType.Outbound, reason);
        _stockTransactions.Add(transaction);

        AddDomainEvent(new StockUpdatedEvent(Id, StockQuantity, quantity, StockTransactionType.Outbound));
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void AssignCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new DomainValidationException("Invalid category ID.");
        CategoryId = categoryId;
    }
}
