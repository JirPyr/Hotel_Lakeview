using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Application.Reservations.Dtos;

/// <summary>
/// DTO varauksen tietojen palauttamiseen.
/// </summary>
public sealed class ReservationDto
{
    /// <summary>
    /// Varauksen tunniste.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Asiakkaan tunniste.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Huoneen tunniste.
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Sisäänkirjautumispäivä.
    /// </summary>
    public DateTime CheckInDate { get; set; }

    /// <summary>
    /// Uloskirjautumispäivä.
    /// </summary>
    public DateTime CheckOutDate { get; set; }

    /// <summary>
    /// Henkilömäärä.
    /// </summary>
    public int GuestCount { get; set; }

    /// <summary>
    /// Kokonaishinta.
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Varauksen tila.
    /// </summary>
    public ReservationStatus Status { get; set; }

    /// <summary>
    /// Lisätiedot.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Luontiaika UTC-aikana.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Viimeisin päivitysaika UTC-aikana.
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Perumisaika UTC-aikana, jos varaus on peruttu.
    /// </summary>
    public DateTime? CancelledAtUtc { get; set; }
}