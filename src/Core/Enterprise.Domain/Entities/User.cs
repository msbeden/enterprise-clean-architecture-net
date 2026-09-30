using Enterprise.Domain.Common;
using Enterprise.Domain.Enums;
using Enterprise.Domain.Events;
using Enterprise.Domain.Exceptions;

namespace Enterprise.Domain.Entities;

public class User : AggregateRoot<Guid>
{
    public string Email { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    private User() : base() { }

    private User(Guid id, string email, string firstName, string lastName, string passwordHash, UserRole role) : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
    }

    public static User Create(string email, string firstName, string lastName, string passwordHash, UserRole role = UserRole.Customer)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainValidationException("Valid email is required.");
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainValidationException("First name cannot be empty.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainValidationException("Last name cannot be empty.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainValidationException("Password hash cannot be empty.");

        var user = new User(Guid.NewGuid(), email.ToLowerInvariant(), firstName, lastName, passwordHash, role);

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.FirstName, user.LastName, user.Role));

        return user;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainValidationException("First name cannot be empty.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainValidationException("Last name cannot be empty.");

        FirstName = firstName;
        LastName = lastName;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainValidationException("Password hash cannot be empty.");

        PasswordHash = passwordHash;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
