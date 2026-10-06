using Enterprise.Domain.Enums;

namespace Enterprise.Application.Features.Users.DTOs;

/// <summary>
/// Data Transfer Object for User entity.
/// </summary>
public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public UserRole Role { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? LastModifiedUtc { get; init; }
}
