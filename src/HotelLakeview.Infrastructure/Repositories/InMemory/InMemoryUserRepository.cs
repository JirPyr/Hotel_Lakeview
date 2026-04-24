using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Infrastructure.Auth;
using System.Reflection;

namespace HotelLakeview.Infrastructure.Repositories;

/// <summary>
/// In-memory-toteutus käyttäjien hakemiseen ja tallentamiseen.
/// </summary>
public sealed class InMemoryUserRepository : IUserRepository
{
    private static readonly List<User> Users = new();
    private static bool _isSeeded;
    private static int _nextId = 1;

    public InMemoryUserRepository()
    {
        SeedUsers();
    }

    public Task<User?> GetByIdAsync(int id)
    {
        var user = Users.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(user);
    }
    public Task<IReadOnlyCollection<User>> GetPagedAsync(int page, int pageSize)
    {
        var items = Users
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<User>)items);
    }

    public Task<int> CountAsync()
    {
        return Task.FromResult(Users.Count);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Task.FromResult<User?>(null);
        }

        var normalizedUsername = username.Trim();

        var user = Users.FirstOrDefault(x =>
            x.Username.Equals(normalizedUsername, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<User> AddAsync(User user)
    {
        SetUserId(user, _nextId++);
        Users.Add(user);

        return Task.FromResult(user);
    }

    public Task<User> UpdateAsync(User user)
    {
        var existingIndex = Users.FindIndex(x => x.Id == user.Id);

        if (existingIndex >= 0)
        {
            Users[existingIndex] = user;
        }

        return Task.FromResult(user);
    }

    private static void SeedUsers()
    {
        if (_isSeeded)
        {
            return;
        }

        var passwordHasher = new PasswordHasher();

        var receptionist = new User(
            "receptionist",
            passwordHasher.Hash("Reception123!"),
            UserRole.Receptionist);

        var management = new User(
            "management",
            passwordHasher.Hash("Management123!"),
            UserRole.Management);

        SetUserId(receptionist, _nextId++);
        SetUserId(management, _nextId++);

        Users.Add(receptionist);
        Users.Add(management);

        _isSeeded = true;
    }

    private static void SetUserId(User user, int id)
    {
        var property = typeof(User).GetProperty(
            nameof(User.Id),
            BindingFlags.Instance | BindingFlags.Public);

        property?.SetValue(user, id);
    }
}