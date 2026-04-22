namespace HotelLakeview.Application.Analytics.Dtos;

/// <summary>
/// DTO yhden kuukauden liikevaihdon palauttamiseen.
/// </summary>
public sealed class MonthlyRevenueDto
{
    /// <summary>
    /// Vuosi.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Kuukausi.
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Kuukauden liikevaihto.
    /// </summary>
    public decimal Revenue { get; set; }
}