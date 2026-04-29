using FluentValidation;

namespace HotelLakeview.Application.Reservations.Commands.CreateReservation;

/// <summary>
/// Validoi uuden varauksen luontikomennon.
/// </summary>
public sealed class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    /// <summary>
    /// Luo validaattorin varauksen luontikomennolle.
    /// </summary>
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("Asiakkaan tunnisteen tulee olla suurempi kuin 0.");

        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("Huoneen tunnisteen tulee olla suurempi kuin 0.");

        RuleFor(x => x.GuestCount)
            .GreaterThan(0)
            .WithMessage("Henkilömäärän tulee olla suurempi kuin 0.");

        RuleFor(x => x.CheckInDate)
            .NotEmpty()
            .WithMessage("Sisäänkirjautumispäivä on pakollinen.");

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Uloskirjautumispäivän tulee olla sisäänkirjautumispäivän jälkeen.");
        
        RuleFor(x => x.CheckInDate)
            .NotEmpty()
            .WithMessage("Check-in date is required.")
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Check-in date cannot be in the past.");
        }
    }
