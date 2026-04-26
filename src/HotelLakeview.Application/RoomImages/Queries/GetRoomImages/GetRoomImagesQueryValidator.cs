using FluentValidation;

namespace HotelLakeview.Application.RoomImages.Queries.GetRoomImages;

/// <summary>
/// Validoi huonekuvien hakukyselyn.
/// </summary>
public sealed class GetRoomImagesQueryValidator
    : AbstractValidator<GetRoomImagesQuery>
{
    public GetRoomImagesQueryValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("Huoneen tunnisteen tulee olla suurempi kuin nolla.");
    }
}