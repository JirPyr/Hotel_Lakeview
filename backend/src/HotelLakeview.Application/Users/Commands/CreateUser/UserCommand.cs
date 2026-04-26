using HotelLakeview.Application.Common.Results;
using MediatR;

namespace HotelLakeview.Application.Users.Commands.CreateUser;

/// <summary>
/// Komento uuden käyttäjän luomiseen.
/// </summary>
public sealed record CreateUserCommand(
    string Username,
    string Password,
    string Role)
    : IRequest<Result>;