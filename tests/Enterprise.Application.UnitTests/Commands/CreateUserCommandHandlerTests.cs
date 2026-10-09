using Enterprise.Application.Common.Interfaces;
using Enterprise.Application.Features.Users.Commands.CreateUser;
using Enterprise.Application.UnitTests.Common;
using Enterprise.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Enterprise.Application.UnitTests.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    public CreateUserCommandHandlerTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock.Setup(p => p.HashPassword(It.IsAny<string>()))
            .Returns("hashed_password_xyz");
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenCommandIsValid()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var handler = new CreateUserCommandHandler(context, _passwordHasherMock.Object);
        var command = new CreateUserCommand("user@test.com", "Password123!", "First", "Last", UserRole.Customer);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == result.Value);
        user.Should().NotBeNull();
        user!.Email.Should().Be("user@test.com");
        user.PasswordHash.Should().Be("hashed_password_xyz");
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserWithEmailAlreadyExists()
    {
        // Arrange
        using var context = TestDbContext.Create();
        var handler = new CreateUserCommandHandler(context, _passwordHasherMock.Object);
        var command = new CreateUserCommand("user@test.com", "Password123!", "First", "Last", UserRole.Customer);

        await handler.Handle(command, CancellationToken.None);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");
    }
}
