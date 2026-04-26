using FluentValidation;

namespace HotelLakeview.Application.RoomImages.Commands.DeleteRoomImage;

/// <summary>
/// Validoi huonekuvan poistokomennon.
/// </summary>
public sealed class DeleteRoomImageCommandValidator
    : AbstractValidator<DeleteRoomImageCommand>
{
    public DeleteRoomImageCommandValidator()
    {
        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage("Kuvan tunnisteen tulee olla suurempi kuin nolla.");
    }
}