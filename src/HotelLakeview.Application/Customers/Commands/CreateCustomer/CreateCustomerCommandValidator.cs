using FluentValidation;

namespace HotelLakeview.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Validoi uuden asiakkaan luontikomennon.
/// </summary>
public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Asiakkaan nimi on pakollinen.")
            .MaximumLength(200)
            .WithMessage("Asiakkaan nimi saa olla enintään 200 merkkiä pitkä.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Sähköpostiosoite ei ole oikeassa muodossa.");

        RuleFor(x => x.PhoneNumber)
            .MinimumLength(5)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Puhelinnumeron tulee olla vähintään 5 merkkiä pitkä.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Puhelinnumero saa olla enintään 20 merkkiä pitkä.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("Lisätiedot saavat olla enintään 1000 merkkiä pitkät.");

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.Email) ||
                !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Asiakkaalla tulee olla joko sähköposti tai puhelinnumero.");
    }
}