using HotelLakeview.Application.Common.Results;
using MediatR;

namespace HotelLakeview.Application.Users.Commands.DeactivateUser;

/// <summary>
/// Komento käyttäjän deaktivoimiseen.
/// </summary>
public sealed record DeactivateUserCommand(int Id) : IRequest<Result>;