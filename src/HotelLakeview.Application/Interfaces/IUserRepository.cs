using HotelLakeview.Domain.Entities;

namespace HotelLakeview.Application.Interfaces;

/// <summary>
/// Määrittelee järjestelmän käyttäjien hakemiseen ja tallentamiseen liittyvät operaatiot.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
}