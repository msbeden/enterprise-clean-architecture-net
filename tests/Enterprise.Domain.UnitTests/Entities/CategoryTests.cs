using Enterprise.Domain.Entities;
using Enterprise.Domain.Events;
using Enterprise.Domain.Exceptions;
using Xunit;

namespace Enterprise.Domain.UnitTests.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateCategoryAndRaiseDomainEvent()
    {
        // Act
        var category = Category.Create("Electronics", "Tech products", 1);

        // Assert
        Assert.NotNull(category);
        Assert.Equal("Electronics", category.Name);
        Assert.Equal("Tech products", category.Description);
        Assert.Equal(1, category.DisplayOrder);
        Assert.True(category.IsActive);
        Assert.Single(category.DomainEvents);
        Assert.IsType<CategoryCreatedEvent>(category.DomainEvents.First());
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainValidationException()
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() => Category.Create("", "Desc", 1));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var category = Category.Create("Books", "Book category", 2);

        // Act
        category.Deactivate();

        // Assert
        Assert.False(category.IsActive);
    }
}
