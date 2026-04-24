namespace HotelLakeview.Application.Auth.Interfaces;

/// <summary>
/// Määrittelee salasanan hashaukseen ja tarkistukseen liittyvät operaatiot.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}