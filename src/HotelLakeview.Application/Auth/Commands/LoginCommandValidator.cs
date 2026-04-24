using FluentValidation;

namespace HotelLakeview.Application.Auth.Commands.Login;

/// <summary>
/// Validoi kirjautumiskomennon syötteet.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(4);
    }
}