using Enterprise.Application.Features.Users.DTOs;

namespace Enterprise.Application.Features.Auth.DTOs;

/// <summary>
/// Response object returned upon successful user authentication.
/// </summary>
public record LoginResponseDto
{
    public string Token { get; init; } = default!;
    public UserDto User { get; init; } = default!;
}
