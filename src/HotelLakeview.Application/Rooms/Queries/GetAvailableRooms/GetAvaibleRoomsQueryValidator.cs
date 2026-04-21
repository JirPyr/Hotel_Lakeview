using FluentValidation;

namespace HotelLakeview.Application.Rooms.Queries.GetAvailableRooms;

/// <summary>
/// Validoi vapaiden huoneiden haun syötteen.
/// </summary>
public class GetAvailableRoomsQueryValidator : AbstractValidator<GetAvailableRoomsQuery>
{
    public GetAvailableRoomsQueryValidator()
    {
        RuleFor(x => x.CheckInDate)
            .NotEmpty()
            .WithMessage("Sisäänkirjautumispäivä on pakollinen.");

        RuleFor(x => x.CheckOutDate)
            .NotEmpty()
            .WithMessage("Uloskirjautumispäivä on pakollinen.");

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Uloskirjautumispäivän on oltava sisäänkirjautumispäivän jälkeen.");

        RuleFor(x => x.GuestCount)
            .GreaterThan(0)
            .WithMessage("Henkilömäärän on oltava vähintään 1.");
    }
}