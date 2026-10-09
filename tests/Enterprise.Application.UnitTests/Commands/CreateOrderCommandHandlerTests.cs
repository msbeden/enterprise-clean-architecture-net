using Enterprise.Application.Features.Orders.Commands.CreateOrder;
using Enterprise.Application.UnitTests.Common;
using Enterprise.Domain.Entities;
using Enterprise.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Enterprise.Application.UnitTests.Commands;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateOrder_WhenUserAndProductsExistWithStock()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var user = User.Create("customer@test.com", "Cust", "Omer", "hashed_password_xyz");
        await context.Users.AddAsync(user);

        var product = Product.Create("Phone", "Smart Phone", SkuCode.Create("PHN-01"), Money.Create(500m, "TRY"), 10);
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var handler = new CreateOrderCommandHandler(context);
        var command = new CreateOrderCommand(
            UserId: user.Id,
            Street: "Ataturk Cad No 1",
            City: "Istanbul",
            State: "Marmara",
            ZipCode: "34000",
            Country: "Turkey",
            Items: new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest(product.Id, 2)
            }
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == result.Value);
        order.Should().NotBeNull();
        order!.TotalAmount.Amount.Should().Be(1000m);

        var updatedProduct = await context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        updatedProduct!.StockQuantity.Should().Be(8);
    }
}
