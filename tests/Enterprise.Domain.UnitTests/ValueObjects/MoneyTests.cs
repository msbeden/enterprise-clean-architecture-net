using Enterprise.Domain.Exceptions;
using Enterprise.Domain.ValueObjects;
using Xunit;

namespace Enterprise.Domain.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateMoneyInstance()
    {
        // Arrange & Act
        var money = Money.Create(100.50m, "USD");

        // Assert
        Assert.Equal(100.50m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowDomainValidationException()
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() => Money.Create(-10m, "USD"));
    }

    [Fact]
    public void Create_WithInvalidCurrencyCode_ShouldThrowDomainValidationException()
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() => Money.Create(50m, "INVALID"));
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnSummedMoney()
    {
        // Arrange
        var m1 = Money.Create(100m, "USD");
        var m2 = Money.Create(50m, "USD");

        // Act
        var result = m1.Add(m2);

        // Assert
        Assert.Equal(150m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ShouldThrowDomainValidationException()
    {
        // Arrange
        var m1 = Money.Create(100m, "USD");
        var m2 = Money.Create(50m, "EUR");

        // Act & Assert
        Assert.Throws<DomainValidationException>(() => m1.Add(m2));
    }

    [Fact]
    public void Subtract_WithValidAmount_ShouldReturnSubtractedMoney()
    {
        // Arrange
        var m1 = Money.Create(100m, "USD");
        var m2 = Money.Create(40m, "USD");

        // Act
        var result = m1.Subtract(m2);

        // Assert
        Assert.Equal(60m, result.Amount);
    }

    [Fact]
    public void Equals_WithSameAmountAndCurrency_ShouldBeEqual()
    {
        // Arrange
        var m1 = Money.Create(100m, "USD");
        var m2 = Money.Create(100m, "USD");

        // Assert
        Assert.Equal(m1, m2);
        Assert.True(m1 == m2);
    }
}
