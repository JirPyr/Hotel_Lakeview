namespace HotelLakeview.Application.RoomImages.Dtos;

/// <summary>
/// Kuvaa huonekuvan tiedot API-vastauksessa.
/// </summary>
public sealed class RoomImageDto
{
    /// <summary>
    /// Kuvan tunniste.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Huoneen tunniste.
    /// </summary>
    public int RoomId { get; init; }

    /// <summary>
    /// Tiedoston nimi.
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// Kuvan URL tai polku.
    /// </summary>
    public string FilePathOrUrl { get; init; } = string.Empty;

    /// <summary>
    /// Tiedoston MIME-tyyppi.
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// Kuvan järjestys huoneessa.
    /// </summary>
    public int SortOrder { get; init; }

    /// <summary>
    /// Onko tämä huoneen pääkuva.
    /// </summary>
    public bool IsPrimary { get; init; }

    /// <summary>
    /// Kuvan latausaika.
    /// </summary>
    public DateTime UploadedAtUtc { get; init; }
}