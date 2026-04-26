using FluentValidation;

namespace HotelLakeview.Application.Reservations.Queries.GetReservationsPaged;

/// <summary>
/// Validoi varausten sivutetun haun syötteen.
/// </summary>
public sealed class GetReservationsPagedQueryValidator : AbstractValidator<GetReservationsPagedQuery>
{
    /// <summary>
    /// Luo validaattorin varausten sivutettua hakua varten.
    /// </summary>
    public GetReservationsPagedQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Sivunumeron tulee olla suurempi kuin 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Sivukoon tulee olla suurempi kuin 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Sivukoko saa olla enintään 100.");
    }
}