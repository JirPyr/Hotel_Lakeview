using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Auth.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using MediatR;

namespace HotelLakeview.Application.Users.Commands.CreateUser;

/// <summary>
/// Käsittelijä käyttäjän luomiseen.
/// </summary>
public sealed class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByUsernameAsync(request.Username);

        if (existing is not null)
        {
            return Result.Failure(
                Error.Conflict("User.UsernameExists", "Username already exists."));
        }

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            return Result.Failure(
                Error.Validation("User.InvalidRole", "Invalid role."));
        }

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(request.Username, passwordHash, role);

        await _userRepository.AddAsync(user);

        return Result.Success();
    }
}