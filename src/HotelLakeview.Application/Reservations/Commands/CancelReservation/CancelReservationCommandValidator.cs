using FluentValidation;

namespace HotelLakeview.Application.Reservations.Commands.CancelReservation;

/// <summary>
/// Validoi varauksen perumiskomennon.
/// </summary>
public sealed class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
{
    /// <summary>
    /// Luo validaattorin varauksen perumiskomennolle.
    /// </summary>
    public CancelReservationCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Varauksen tunnisteen tulee olla suurempi kuin 0.");
    }
}