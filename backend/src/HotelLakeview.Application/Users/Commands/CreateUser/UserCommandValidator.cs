using FluentValidation;

namespace HotelLakeview.Application.Users.Commands.CreateUser;

/// <summary>
/// Validoi käyttäjän luontikomennon.
/// </summary>
public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => r == "Receptionist" || r == "Management")
            .WithMessage("Role must be Receptionist or Management.");
    }
}