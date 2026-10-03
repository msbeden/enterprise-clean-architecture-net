using Enterprise.Domain.Entities;
using Enterprise.Domain.Enums;
using Enterprise.Domain.Events;
using Enterprise.Domain.Exceptions;
using Xunit;

namespace Enterprise.Domain.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateUserAndRaiseDomainEvent()
    {
        // Act
        var user = User.Create("john.doe@example.com", "John", "Doe", "hashed_password_123", UserRole.Customer);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("john.doe@example.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("hashed_password_123", user.PasswordHash);
        Assert.Equal(UserRole.Customer, user.Role);
        Assert.True(user.IsActive);
        Assert.Single(user.DomainEvents);
        Assert.IsType<UserRegisteredEvent>(user.DomainEvents.First());
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldThrowDomainValidationException()
    {
        // Act & Assert
        Assert.Throws<DomainValidationException>(() => User.Create("invalid-email", "John", "Doe", "hashed_password_123"));
    }

    [Fact]
    public void UpdateProfile_WithValidName_ShouldUpdateProperties()
    {
        // Arrange
        var user = User.Create("jane.doe@example.com", "Jane", "Doe", "hashed_password_123");

        // Act
        user.UpdateProfile("Janet", "Smith");

        // Assert
        Assert.Equal("Janet", user.FirstName);
        Assert.Equal("Smith", user.LastName);
    }
}
