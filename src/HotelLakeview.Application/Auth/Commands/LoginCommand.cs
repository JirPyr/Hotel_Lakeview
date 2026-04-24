using HotelLakeview.Application.Auth.Dtos;
using HotelLakeview.Application.Common.Results;
using MediatR;

namespace HotelLakeview.Application.Auth.Commands.Login;

/// <summary>
/// Komento käyttäjän kirjautumiseen.
/// </summary>
public sealed record LoginCommand(
    string Username,
    string Password)
    : IRequest<Result<LoginResponseDto>>;