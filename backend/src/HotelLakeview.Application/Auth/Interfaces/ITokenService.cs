using HotelLakeview.Domain.Entities;

namespace HotelLakeview.Application.Auth.Interfaces;

/// <summary>
/// Määrittelee käyttöoikeustokenin luomiseen liittyvät operaatiot.
/// </summary>
public interface ITokenService
{
    string CreateToken(User user);
}