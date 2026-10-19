using System.Net;
using System.Net.Http.Json;
using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Auth.Commands.Login;
using Enterprise.Application.Features.Auth.DTOs;
using Enterprise.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Enterprise.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidUserAndCorrectPassword_ShouldReturn200OKAndJwtToken()
    {
        // 1. Arrange - Seed user using factory (default password: Password123!)
        _factory.CreateAuthenticatedClient("loginuser@example.com", UserRole.Customer);
        var loginCommand = new LoginCommand("loginuser@example.com", "Password123!");

        // 2. Act - Call Login endpoint anonymously
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginCommand);

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Result<LoginResponseDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Token.Should().NotBeNullOrEmpty();
        result.Value.User.Email.Should().Be("loginuser@example.com");
    }

    [Fact]
    public async Task Login_WithValidUserAndIncorrectPassword_ShouldReturn400BadRequest()
    {
        // Arrange
        _factory.CreateAuthenticatedClient("loginuser2@example.com", UserRole.Customer);
        var loginCommand = new LoginCommand("loginuser2@example.com", "WrongPassword!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Result<LoginResponseDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid email credentials or password");
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ShouldReturn400BadRequest()
    {
        // Arrange
        var loginCommand = new LoginCommand("nonexistent.user@example.com", "Password123!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Result<LoginResponseDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid email credentials");
    }
}
