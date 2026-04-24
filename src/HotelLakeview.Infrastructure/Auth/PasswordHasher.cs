using System.Security.Cryptography;
using System.Text;
using HotelLakeview.Application.Auth.Interfaces;

namespace HotelLakeview.Infrastructure.Auth;

/// <summary>
/// Salasanojen hashaukseen käytettävä toteutus.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool Verify(string password, string passwordHash)
    {
        var hashedInput = Hash(password);
        return hashedInput == passwordHash;
    }
}