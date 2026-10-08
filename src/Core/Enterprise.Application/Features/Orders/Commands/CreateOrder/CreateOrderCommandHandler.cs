using Enterprise.Application.Common.Interfaces;
using Enterprise.Application.Common.Models;
using Enterprise.Domain.Entities;
using Enterprise.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Handler for CreateOrderCommand.
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == request.UserId && u.IsActive, cancellationToken);

        if (!userExists)
        {
            return Result<Guid>.Failure($"Active user with ID '{request.UserId}' was not found.");
        }

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var itemReq in request.Items)
        {
            if (!products.TryGetValue(itemReq.ProductId, out var product))
            {
                return Result<Guid>.Failure($"Product with ID '{itemReq.ProductId}' was not found.");
            }

            if (!product.IsActive)
            {
                return Result<Guid>.Failure($"Product '{product.Name}' is not active.");
            }

            if (product.StockQuantity < itemReq.Quantity)
            {
                return Result<Guid>.Failure($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemReq.Quantity}.");
            }
        }

        var address = new Address(
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Country
        );

        var orderItems = new List<OrderItem>();
        foreach (var itemReq in request.Items)
        {
            var product = products[itemReq.ProductId];
            
            // Deduct stock for order creation
            product.DecreaseStock(itemReq.Quantity, "Order creation");

            var orderItem = OrderItem.Create(
                product.Id,
                product.Name,
                product.Price,
                itemReq.Quantity
            );
            orderItems.Add(orderItem);
        }

        var order = Order.Create(request.UserId, address, orderItems);

        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(order.Id);
    }
}
