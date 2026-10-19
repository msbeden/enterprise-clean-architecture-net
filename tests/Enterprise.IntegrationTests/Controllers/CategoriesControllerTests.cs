using System.Net;
using System.Net.Http.Json;
using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Categories.Commands.CreateCategory;
using Enterprise.Application.Features.Categories.DTOs;
using FluentAssertions;
using Xunit;

namespace Enterprise.IntegrationTests.Controllers;

public class CategoriesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_WithValidCommand_ShouldReturn200OKAndCategoryId()
    {
        // Arrange
        var command = new CreateCategoryCommand("Hardware", "PC Hardware", 1);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Categories", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturn200OKAndCategoryList()
    {
        // Act
        var response = await _client.GetAsync("/api/Categories");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Result<List<CategoryDto>>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
    }
}
