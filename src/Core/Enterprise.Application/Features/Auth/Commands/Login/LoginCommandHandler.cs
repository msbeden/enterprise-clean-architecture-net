using Enterprise.Application.Common.Interfaces;
using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Auth.DTOs;
using Enterprise.Application.Features.Users.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handler for LoginCommand.
/// Validates user email, password, and activity state, then produces a JWT Bearer token.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return Result<LoginResponseDto>.Failure("Invalid email credentials or user account is inactive.");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<LoginResponseDto>.Failure("Invalid email credentials or password.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        var userDto = user.Adapt<UserDto>();

        var response = new LoginResponseDto
        {
            Token = token,
            User = userDto
        };

        return Result<LoginResponseDto>.Success(response);
    }
}
