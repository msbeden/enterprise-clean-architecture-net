using Enterprise.Application.Features.Products.Commands.CreateProduct;
using Enterprise.Application.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Enterprise.Application.UnitTests.Commands;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenCommandIsValid()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var handler = new CreateProductCommandHandler(context);
        var command = new CreateProductCommand(
            Name: "Test Laptop",
            Description: "High performance laptop",
            Sku: "LAP-001",
            Price: 1499.99m,
            Currency: "USD",
            InitialStock: 10
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.Id == result.Value);
        savedProduct.Should().NotBeNull();
        savedProduct!.Name.Should().Be("Test Laptop");
        savedProduct.Sku.Value.Should().Be("LAP-001");
        savedProduct.Price.Amount.Should().Be(1499.99m);
        savedProduct.StockQuantity.Should().Be(10);
    }

    [Theory]
    [InlineData("", "SKU-01", 100, "USD", 5, "Product name is required.")]
    [InlineData("Laptop", "", 100, "USD", 5, "SKU code is required.")]
    [InlineData("Laptop", "SKU-01", -10, "USD", 5, "Price must be greater than zero.")]
    [InlineData("Laptop", "SKU-01", 100, "INVALID", 5, "Currency code must be exactly 3 characters")]
    public void Validator_ShouldFail_WhenCommandHasInvalidFields(
        string name, string sku, decimal price, string currency, int stock, string expectedErrorSubstring)
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand(name, "Desc", sku, price, currency, stock);

        // Act
        var validationResult = validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedErrorSubstring));
    }
}
