using HotelLakeview.Application.Auth.Dtos;
using HotelLakeview.Application.Auth.Interfaces;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using MediatR;

namespace HotelLakeview.Application.Auth.Commands.Login;

/// <summary>
/// Käsittelijä käyttäjän kirjautumiseen.
/// </summary>
public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user is null || !user.IsActive)
        {
            return Result<LoginResponseDto>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Virheellinen käyttäjätunnus tai salasana."));
        }

        var passwordIsValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!passwordIsValid)
        {
            return Result<LoginResponseDto>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Virheellinen käyttäjätunnus tai salasana."));
        }

        var token = _tokenService.CreateToken(user);

        var response = new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString()
        };

        return Result<LoginResponseDto>.Success(response);
    }
}