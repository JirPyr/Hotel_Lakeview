namespace HotelLakeview.Application.Analytics.Dtos;

/// <summary>
/// DTO liikevaihtoraportin palauttamiseen.
/// </summary>
public sealed class RevenueReportDto
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
    /// Kuukausittaiset liikevaihtorivit.
    /// </summary>
    public IReadOnlyCollection<MonthlyRevenueDto> Months { get; set; } = [];

    /// <summary>
    /// Kokonaisliikevaihto raportointijaksolta.
    /// </summary>
    public decimal TotalRevenue { get; set; }
}