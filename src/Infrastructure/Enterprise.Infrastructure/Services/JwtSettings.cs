namespace Enterprise.Infrastructure.Services;

/// <summary>
/// Configuration options for JSON Web Token (JWT) generation and validation.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Secret { get; init; } = "EnterpriseCleanArchitecture_SecretKey_987654321_ForJwtTokenGeneration!";
    public string Issuer { get; init; } = "EnterpriseCleanArchitecture";
    public string Audience { get; init; } = "EnterpriseCleanArchitectureUsers";
    public int ExpirationInMinutes { get; init; } = 60;
}
