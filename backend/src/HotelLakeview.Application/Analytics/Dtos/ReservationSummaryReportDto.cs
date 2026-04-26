namespace HotelLakeview.Application.Analytics.Dtos;

/// <summary>
/// Kuvaa varausten yhteenvedon raportointia varten.
/// Sisältää sekä aktiiviset että perutut varaukset.
/// </summary>
public sealed class ReservationSummaryReportDto
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    /// <summary>
    /// Aktiivisten varausten määrä aikavälillä.
    /// </summary>
    public int ActiveReservationCount { get; init; }

    /// <summary>
    /// Peruttujen varausten määrä aikavälillä.
    /// </summary>
    public int CancelledReservationCount { get; init; }

    /// <summary>
    /// Varausten kokonaismäärä.
    /// </summary>
    public int TotalReservationCount { get; init; }

    /// <summary>
    /// Peruutusaste prosentteina.
    /// </summary>
    public decimal CancellationRatePercentage { get; init; }

    /// <summary>
    /// Peruttujen varausten yhteenlaskettu arvo.
    /// </summary>
    public decimal CancelledRevenueValue { get; init; }
}