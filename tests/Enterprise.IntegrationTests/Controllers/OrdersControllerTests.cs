using System.Net;
using System.Net.Http.Json;
using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Orders.Commands.CreateOrder;
using Enterprise.Application.Features.Orders.DTOs;
using Enterprise.Application.Features.Products.Commands.CreateProduct;
using Enterprise.Application.Features.Users.Commands.CreateUser;
using FluentAssertions;
using Xunit;

namespace Enterprise.IntegrationTests.Controllers;

public class OrdersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _anonymousClient;
    private readonly HttpClient _authenticatedClient;

    public OrdersControllerTests(CustomWebApplicationFactory factory)
    {
        _anonymousClient = factory.CreateClient();
        _authenticatedClient = factory.CreateAuthenticatedClient("order.tester@example.com");
    }

    [Fact]
    public async Task Request_WithoutToken_ShouldReturn401Unauthorized()
    {
        // Act
        var response = await _anonymousClient.GetAsync($"/api/Orders/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithValidData_WhenAuthenticated_ShouldReturn201CreatedAndGetByIdShouldWork()
    {
        // 1. Create User via authenticated client
        var createUserCmd = new CreateUserCommand("order.buyer@example.com", "Password123!", "Order", "Buyer");
        var userRes = await _authenticatedClient.PostAsJsonAsync("/api/Users", createUserCmd);
        var userId = (await userRes.Content.ReadFromJsonAsync<Result<Guid>>())!.Value;

        // 2. Create Product (Public endpoint, works with anonymous client or auth client)
        var createProdCmd = new CreateProductCommand("Order Item Product", "Desc", "ORD-PRD-01", 100m, "TRY", 20);
        var prodRes = await _anonymousClient.PostAsJsonAsync("/api/Products", createProdCmd);
        var prodId = (await prodRes.Content.ReadFromJsonAsync<Result<Guid>>())!.Value;

        // 3. Create Order via authenticated client
        var createOrderCmd = new CreateOrderCommand(
            UserId: userId,
            Street: "Istiklal Cad",
            City: "Istanbul",
            State: "Marmara",
            ZipCode: "34000",
            Country: "Turkey",
            Items: new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest(prodId, 2)
            }
        );

        var orderRes = await _authenticatedClient.PostAsJsonAsync("/api/Orders", createOrderCmd);
        orderRes.StatusCode.Should().Be(HttpStatusCode.Created);

        var orderId = (await orderRes.Content.ReadFromJsonAsync<Result<Guid>>())!.Value;
        orderId.Should().NotBeEmpty();

        // 4. Get Order by Id via authenticated client
        var getOrderRes = await _authenticatedClient.GetAsync($"/api/Orders/{orderId}");
        getOrderRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var orderDtoResult = await getOrderRes.Content.ReadFromJsonAsync<Result<OrderDto>>();
        orderDtoResult.Should().NotBeNull();
        orderDtoResult!.IsSuccess.Should().BeTrue();
        orderDtoResult.Value!.TotalAmount.Should().Be(200m);
        orderDtoResult.Value.Items.Should().HaveCount(1);
    }
}
