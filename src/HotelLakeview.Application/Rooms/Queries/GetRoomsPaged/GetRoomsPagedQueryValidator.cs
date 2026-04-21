using FluentValidation;

namespace HotelLakeview.Application.Rooms.Queries.GetRoomsPaged;

/// <summary>
/// Validoi huoneiden sivutetun haun syötteen.
/// </summary>
public sealed class GetRoomsPagedQueryValidator : AbstractValidator<GetRoomsPagedQuery>
{
    /// <summary>
    /// Luo validaattorin huoneiden sivutettua hakua varten.
    /// </summary>
    public GetRoomsPagedQueryValidator()
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