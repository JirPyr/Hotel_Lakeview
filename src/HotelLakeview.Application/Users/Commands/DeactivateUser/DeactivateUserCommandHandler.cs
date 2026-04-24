using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using MediatR;

namespace HotelLakeview.Application.Users.Commands.DeactivateUser;

/// <summary>
/// Käsittelijä käyttäjän deaktivoimiseen.
/// </summary>
public sealed class DeactivateUserCommandHandler
    : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public DeactivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(
        DeactivateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", "Käyttäjää ei löytynyt."));
        }

        if (!user.IsActive)
        {
            return Result.Failure(
                Error.Conflict("User.AlreadyInactive", "Käyttäjä on jo passiivinen."));
        }

        user.Deactivate();

        await _userRepository.UpdateAsync(user);

        return Result.Success();
    }
}