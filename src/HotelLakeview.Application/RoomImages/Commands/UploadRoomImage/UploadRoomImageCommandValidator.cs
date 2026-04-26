using FluentValidation;

namespace HotelLakeview.Application.RoomImages.Commands.UploadRoomImage;

/// <summary>
/// Validoi huonekuvan latauskomennon.
/// </summary>
public sealed class UploadRoomImageCommandValidator
    : AbstractValidator<UploadRoomImageCommand>
{
    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    /// <summary>
    /// Luo uuden validaattorin huonekuvan lataukselle.
    /// </summary>
    public UploadRoomImageCommandValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("Huoneen tunnisteen tulee olla suurempi kuin nolla.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("Tiedostonimi on pakollinen.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Tiedoston content type on pakollinen.")
            .Must(contentType => AllowedContentTypes.Contains(contentType))
            .WithMessage("Sallittuja tiedostotyyppejä ovat image/jpeg, image/png ja image/webp.");

        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("Tiedoston sisältö on pakollinen.");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Kuvan järjestysnumero ei voi olla negatiivinen.");
    }
}