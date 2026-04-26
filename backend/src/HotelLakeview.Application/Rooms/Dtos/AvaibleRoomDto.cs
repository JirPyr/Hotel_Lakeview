using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Application.Rooms.Dtos;

/// <summary>
/// DTO vapaiden huoneiden palauttamiseen aikavälihaussa.
/// </summary>
public class AvailableRoomDto
{
    /// <summary>
    /// Huoneen tunniste.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Huoneen numero.
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Huoneen tyyppi.
    /// </summary>
    public RoomType RoomType { get; set; }

    /// <summary>
    /// Huoneen suurin sallittu henkilömäärä.
    /// </summary>
    public int MaxGuests { get; set; }

    /// <summary>
    /// Huoneen perushinta per yö.
    /// </summary>
    public decimal BasePricePerNight { get; set; }

    /// <summary>
    /// Huoneen kuvaus.
    /// </summary>
    public string? Description { get; set; }
}