#nullable enable
using System;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class User : AuditableEntity<Guid>
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public int LoyaltyPoints { get; private set; }
    public bool IsActive { get; private set; }

    private User() { } // For EF Core

    public User(string fullName, string email, string passwordHash, string? phoneNumber)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        LoyaltyPoints = 0;
        IsActive = true;
    }

    public void AddLoyaltyPoints(int points)
    {
        if (points > 0)
            LoyaltyPoints += points;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
