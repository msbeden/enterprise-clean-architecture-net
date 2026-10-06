using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Auth.DTOs;
using MediatR;

namespace Enterprise.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user by email and password to retrieve a JWT token.
/// </summary>
public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponseDto>>;
