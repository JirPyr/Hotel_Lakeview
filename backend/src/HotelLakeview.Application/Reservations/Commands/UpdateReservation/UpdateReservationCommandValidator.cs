using FluentValidation;

namespace HotelLakeview.Application.Reservations.Commands.UpdateReservation;

/// <summary>
/// Validointi varauksen päivittämiselle.
/// </summary>
public sealed class UpdateReservationCommandValidator
    : AbstractValidator<UpdateReservationCommand>
{
    public UpdateReservationCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Reservation id must be greater than 0.");

        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("Room id must be greater than 0.");

        RuleFor(x => x.GuestCount)
            .GreaterThan(0)
            .WithMessage("Guest count must be greater than 0.");

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out date must be later than check-in date.");
        RuleFor(x => x.CheckInDate)
            .NotEmpty()
            .WithMessage("Check-in date is required.")
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Check-in date cannot be in the past.");
        }
}