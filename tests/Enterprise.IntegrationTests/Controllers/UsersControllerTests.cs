using System.Net;
using System.Net.Http.Json;
using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Users.Commands.CreateUser;
using Enterprise.Application.Features.Users.DTOs;
using Enterprise.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Enterprise.IntegrationTests.Controllers;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _anonymousClient;
    private readonly HttpClient _authenticatedClient;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _anonymousClient = factory.CreateClient();
        _authenticatedClient = factory.CreateAuthenticatedClient("admin.user@example.com", UserRole.Admin);
    }

    [Fact]
    public async Task Request_WithoutToken_ShouldReturn401Unauthorized()
    {
        // Act
        var response = await _anonymousClient.GetAsync("/api/Users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithValidCommand_WhenAuthenticated_ShouldReturn200OKAndUserId()
    {
        // Arrange
        var command = new CreateUserCommand("integration.user@example.com", "Password123!", "Integration", "User", UserRole.Customer);

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/api/Users", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_WhenAuthenticated_ShouldReturn200OKAndUserList()
    {
        // Act
        var response = await _authenticatedClient.GetAsync("/api/Users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Result<List<UserDto>>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
    }
}
