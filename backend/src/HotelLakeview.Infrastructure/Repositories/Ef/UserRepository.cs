using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Repositories.Ef;

/// <summary>
/// EF Core -toteutus käyttäjien hakemiseen ja tallentamiseen.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly HotelLakeviewDbContext _dbContext;

    public UserRepository(HotelLakeviewDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        var normalizedUsername = username.Trim();

        return await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Username == normalizedUsername);
    }
    public async Task<IReadOnlyCollection<User>> GetPagedAsync(int page, int pageSize)
    {
        var items = await _dbContext.Users
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return items.AsReadOnly();
    }

    public async Task<int> CountAsync()
    {
        return await _dbContext.Users.CountAsync();
    }

    public async Task<User> AddAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}