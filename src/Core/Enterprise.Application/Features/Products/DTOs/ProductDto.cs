namespace Enterprise.Application.Features.Products.DTOs;

/// <summary>
/// Data Transfer Object for Product entity.
/// </summary>
public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string Sku { get; init; } = default!;
    public decimal Price { get; init; }
    public string Currency { get; init; } = default!;
    public int StockQuantity { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? LastModifiedUtc { get; init; }
}
