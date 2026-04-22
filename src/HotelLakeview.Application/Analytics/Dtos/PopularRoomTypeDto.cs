using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Application.Analytics.Dtos;

/// <summary>
/// DTO suositun huonetyypin tietojen palauttamiseen.
/// </summary>
public sealed class PopularRoomTypeDto
{
    /// <summary>
    /// Huonetyyppi.
    /// </summary>
    public RoomType RoomType { get; set; }

    /// <summary>
    /// Varausten määrä.
    /// </summary>
    public int ReservationCount { get; set; }

    /// <summary>
    /// Osuus kaikista varauksista prosentteina.
    /// </summary>
    public decimal PercentageOfReservations { get; set; }
}