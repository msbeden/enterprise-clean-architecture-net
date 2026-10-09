using Enterprise.Application.Features.Categories.Commands.CreateCategory;
using Enterprise.Application.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Enterprise.Application.UnitTests.Commands;

public class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateCategory_WhenCommandIsValid()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var handler = new CreateCategoryCommandHandler(context);
        var command = new CreateCategoryCommand("Software", "Software products", 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == result.Value);
        category.Should().NotBeNull();
        category!.Name.Should().Be("Software");
        category.DisplayOrder.Should().Be(1);
    }
}
