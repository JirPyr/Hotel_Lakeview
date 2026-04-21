using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Domain.Entities;

/// <summary>
/// Kuvaa järjestelmän käyttäjää.
/// </summary>
public class User
{
    private User()
    {
    }

    public User(string username, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        Username = username.Trim();
        PasswordHash = passwordHash.Trim();
        Role = role;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public void Deactivate()
    {
        IsActive = false;
    }
}