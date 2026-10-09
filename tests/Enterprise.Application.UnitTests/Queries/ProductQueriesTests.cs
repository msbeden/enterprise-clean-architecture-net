using Enterprise.Application.Features.Products.Queries.GetProductById;
using Enterprise.Application.Features.Products.Queries.GetProductsWithPagination;
using Enterprise.Application.Mappings;
using Enterprise.Application.UnitTests.Common;
using Enterprise.Domain.Entities;
using Enterprise.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Enterprise.Application.UnitTests.Queries;

public class ProductQueriesTests
{
    public ProductQueriesTests()
    {
        new MappingConfig().Register(Mapster.TypeAdapterConfig.GlobalSettings);
    }
    [Fact]
    public async Task GetProductById_ShouldReturnProductDto_WhenProductExists()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var product = Product.Create("Phone", "Smart Phone", SkuCode.Create("PHN-100"), Money.Create(799m, "USD"), 15);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new GetProductByIdQueryHandler(context);
        var query = new GetProductByIdQuery(product.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(product.Id);
        result.Value.Name.Should().Be("Phone");
        result.Value.Sku.Should().Be("PHN-100");
        result.Value.Price.Should().Be(799m);
        result.Value.Currency.Should().Be("USD");
    }

    [Fact]
    public async Task GetProductById_ShouldReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var handler = new GetProductByIdQueryHandler(context);
        var query = new GetProductByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("was not found");
    }

    [Fact]
    public async Task GetProductsWithPagination_ShouldReturnFilteredAndPaginatedResults()
    {
        // Arrange
        using var context = TestDbContext.Create();
        context.Products.AddRange(
            Product.Create("Alpha Book", "Reading", SkuCode.Create("BK-001"), Money.Create(20, "USD"), 5),
            Product.Create("Beta Laptop", "Computing", SkuCode.Create("LP-002"), Money.Create(1000, "USD"), 2),
            Product.Create("Gamma Keyboard", "Accessories", SkuCode.Create("KB-003"), Money.Create(50, "USD"), 10)
        );
        await context.SaveChangesAsync();

        var handler = new GetProductsWithPaginationQueryHandler(context);
        var query = new GetProductsWithPaginationQuery(PageNumber: 1, PageSize: 2, SearchTerm: null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Count.Should().Be(2);
        result.Value.TotalCount.Should().Be(3);
        result.Value.TotalPages.Should().Be(2);
        result.Value.HasNextPage.Should().BeTrue();
        result.Value.HasPreviousPage.Should().BeFalse();
    }
}
