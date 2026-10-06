using Enterprise.Application.Common.Models;
using Enterprise.Domain.Enums;
using MediatR;

namespace Enterprise.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Command to create a new user with password credentials.
/// </summary>
public record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role = UserRole.Customer
) : IRequest<Result<Guid>>;
