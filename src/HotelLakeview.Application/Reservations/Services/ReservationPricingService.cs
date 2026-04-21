namespace HotelLakeview.Application.Reservations.Services;

/// <summary>
/// Laskee varauksen kokonaishinnan yöpohjaisesti sesonkihinnoittelu huomioiden.
/// </summary>
public sealed class ReservationPricingService : IReservationPricingService
{
    private const decimal SeasonalMultiplier = 1.30m;

    /// <inheritdoc />
    public decimal CalculateTotalPrice(
        decimal basePricePerNight,
        DateTime checkInDate,
        DateTime checkOutDate)
    {
        if (checkOutDate <= checkInDate)
        {
            return 0m;
        }

        decimal total = 0m;

        for (var date = checkInDate.Date; date < checkOutDate.Date; date = date.AddDays(1))
        {
            var nightlyPrice = IsSeasonalDate(date)
                ? basePricePerNight * SeasonalMultiplier
                : basePricePerNight;

            total += nightlyPrice;
        }

        return decimal.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Tarkistaa, kuuluuko päivä sesonkiin.
    /// </summary>
    private static bool IsSeasonalDate(DateTime date)
    {
        var isSummerSeason = date.Month is 6 or 7 or 8;

        var isChristmasSeason =
            (date.Month == 12 && date.Day >= 20) ||
            (date.Month == 1 && date.Day <= 6);

        return isSummerSeason || isChristmasSeason;
    }
}