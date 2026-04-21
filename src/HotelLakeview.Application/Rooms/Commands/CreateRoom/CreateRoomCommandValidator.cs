using FluentValidation;

namespace HotelLakeview.Application.Rooms.Commands.CreateRoom;

/// <summary>
/// Validoi uuden huoneen luontikomennon.
/// </summary>
public sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    /// <summary>
    /// Luo validaattorin huoneen luontia varten.
    /// </summary>
    public CreateRoomCommandValidator()
    {
        RuleFor(x => x.RoomNumber)
            .NotEmpty()
            .WithMessage("Huonenumero on pakollinen.")
            .MaximumLength(20)
            .WithMessage("Huonenumero saa olla enintään 20 merkkiä pitkä.");

        RuleFor(x => x.MaxGuests)
            .GreaterThan(0)
            .WithMessage("Maksimihenkilömäärän tulee olla suurempi kuin 0.");

        RuleFor(x => x.BasePricePerNight)
            .GreaterThan(0)
            .WithMessage("Perushinnan per yö tulee olla suurempi kuin 0.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Kuvaus saa olla enintään 1000 merkkiä pitkä.");
    }
}