using System.Net;
using System.Net.Http.Json;
using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Products.Commands.CreateProduct;
using Enterprise.Application.Features.Products.DTOs;
using FluentAssertions;
using Xunit;

namespace Enterprise.IntegrationTests.Controllers;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_WithValidCommand_ShouldReturn201CreatedAndProductId()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Integration Test Laptop",
            Description: "High specs",
            Sku: "IT-LAP-001",
            Price: 1999.99m,
            Currency: "USD",
            InitialStock: 5
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products", command);

        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, because: content);

        var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_WhenProductExists_ShouldReturn200OKAndProductDetails()
    {
        // Arrange: First create a product
        var createCommand = new CreateProductCommand(
            Name: "Integration Monitor",
            Description: "4K Monitor",
            Sku: "IT-MNT-002",
            Price: 499.00m,
            Currency: "USD",
            InitialStock: 12
        );
        var createResponse = await _client.PostAsJsonAsync("/api/Products", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Result<Guid>>();
        var productId = createResult!.Value;

        // Act: Fetch by ID
        var getResponse = await _client.GetAsync($"/api/Products/{productId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResult = await getResponse.Content.ReadFromJsonAsync<Result<ProductDto>>();
        getResult.Should().NotBeNull();
        getResult!.IsSuccess.Should().BeTrue();
        getResult.Value.Should().NotBeNull();
        getResult.Value!.Id.Should().Be(productId);
        getResult.Value.Name.Should().Be("Integration Monitor");
        getResult.Value.Sku.Should().Be("IT-MNT-002");
        getResult.Value.Price.Should().Be(499.00m);
    }

    [Fact]
    public async Task GetById_WhenProductDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/Products/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPaginated_ShouldReturn200OKWithPaginatedList()
    {
        // Act
        var response = await _client.GetAsync("/api/Products?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<PaginatedList<ProductDto>>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_WithInvalidCommand_ShouldReturn400BadRequest()
    {
        // Arrange: Empty name and negative price
        var invalidCommand = new CreateProductCommand(
            Name: "",
            Description: "Desc",
            Sku: "SKU-999",
            Price: -50m,
            Currency: "USD",
            InitialStock: 5
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products", invalidCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
