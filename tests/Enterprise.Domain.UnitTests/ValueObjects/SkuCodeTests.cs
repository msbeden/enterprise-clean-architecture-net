using Enterprise.Domain.Exceptions;
using Enterprise.Domain.ValueObjects;
using Xunit;

namespace Enterprise.Domain.UnitTests.ValueObjects;

public class SkuCodeTests
{
    [Fact]
    public void Create_WithValidCode_ShouldCreateSkuCode()
    {
        // Act
        var sku = SkuCode.Create("  prd-123  ");

        // Assert
        Assert.Equal("PRD-123", sku.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("AB")]
    public void Create_WithInvalidCode_ShouldThrowDomainValidationException(string code)
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() => SkuCode.Create(code));
    }
}
