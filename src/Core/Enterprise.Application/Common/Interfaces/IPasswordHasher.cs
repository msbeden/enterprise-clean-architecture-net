namespace Enterprise.Application.Common.Interfaces;

/// <summary>
/// Service interface for securely hashing and verifying passwords.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
