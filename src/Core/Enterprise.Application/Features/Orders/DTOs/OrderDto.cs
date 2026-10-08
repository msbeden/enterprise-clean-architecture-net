namespace Enterprise.Application.Features.Orders.DTOs;

public record AddressDto
{
    public string Street { get; init; } = default!;
    public string City { get; init; } = default!;
    public string State { get; init; } = default!;
    public string ZipCode { get; init; } = default!;
    public string Country { get; init; } = default!;
}

public record OrderItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = default!;
    public decimal UnitPrice { get; init; }
    public string Currency { get; init; } = default!;
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
}

public record OrderDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public AddressDto ShippingAddress { get; init; } = default!;
    public string Status { get; init; } = default!;
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = default!;
    public List<OrderItemDto> Items { get; init; } = new();
    public DateTime CreatedUtc { get; init; }
}
