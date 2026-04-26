using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Application.Rooms.Dtos;

/// <summary>
/// DTO huoneen tietojen palauttamiseen.
/// </summary>
public class RoomDto
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

    /// <summary>
    /// Kertoo, onko huone aktiivinen.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Huoneen luontiaika UTC-aikana.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Huoneen viimeisin päivitysaika UTC-aikana.
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; }
}