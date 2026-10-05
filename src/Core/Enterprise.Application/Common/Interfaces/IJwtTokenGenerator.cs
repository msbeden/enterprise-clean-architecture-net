using Enterprise.Domain.Entities;

namespace Enterprise.Application.Common.Interfaces;

/// <summary>
/// Service interface for generating JSON Web Tokens (JWT).
/// Implemented in Infrastructure layer.
/// </summary>
public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
