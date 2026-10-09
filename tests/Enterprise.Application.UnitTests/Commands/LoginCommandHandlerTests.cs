using Enterprise.Application.Common.Interfaces;
using Enterprise.Application.Features.Auth.Commands.Login;
using Enterprise.Application.UnitTests.Common;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace Enterprise.Application.UnitTests.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    public LoginCommandHandlerTests()
    {
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns("mock-jwt-token-12345");

        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock
            .Setup(x => x.VerifyPassword("ValidPassword123!", "hashed_pass"))
            .Returns(true);
        _passwordHasherMock
            .Setup(x => x.VerifyPassword(It.Is<string>(p => p != "ValidPassword123!"), It.IsAny<string>()))
            .Returns(false);
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var user = User.Create("authuser@test.com", "Auth", "User", "hashed_pass", UserRole.Customer);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var handler = new LoginCommandHandler(context, _jwtTokenGeneratorMock.Object, _passwordHasherMock.Object);
        var command = new LoginCommand("authuser@test.com", "ValidPassword123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().Be("mock-jwt-token-12345");
        result.Value.User.Email.Should().Be("authuser@test.com");
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPasswordIsIncorrect()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var user = User.Create("authuser@test.com", "Auth", "User", "hashed_pass", UserRole.Customer);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var handler = new LoginCommandHandler(context, _jwtTokenGeneratorMock.Object, _passwordHasherMock.Object);
        var command = new LoginCommand("authuser@test.com", "WrongPassword!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid email credentials or password");
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var handler = new LoginCommandHandler(context, _jwtTokenGeneratorMock.Object, _passwordHasherMock.Object);
        var command = new LoginCommand("nonexistent@test.com", "ValidPassword123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid email credentials");
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserIsInactive()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var user = User.Create("inactiveuser@test.com", "Inactive", "User", "hashed_pass", UserRole.Customer);
        user.Deactivate();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var handler = new LoginCommandHandler(context, _jwtTokenGeneratorMock.Object, _passwordHasherMock.Object);
        var command = new LoginCommand("inactiveuser@test.com", "ValidPassword123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("inactive");
    }
}
