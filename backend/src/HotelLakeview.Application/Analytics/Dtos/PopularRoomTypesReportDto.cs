namespace HotelLakeview.Application.Analytics.Dtos;

/// <summary>
/// DTO suosituimpien huonetyyppien raportin palauttamiseen.
/// </summary>
public sealed class PopularRoomTypesReportDto
{
    /// <summary>
    /// Raportin alkupäivä.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Raportin loppupäivä.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Mukaan laskettujen varausten kokonaismäärä.
    /// </summary>
    public int TotalReservationCount { get; set; }

    /// <summary>
    /// Huonetyyppikohtaiset rivit suosituimmuusjärjestyksessä.
    /// </summary>
    public IReadOnlyCollection<PopularRoomTypeDto> RoomTypes { get; set; } = [];
}