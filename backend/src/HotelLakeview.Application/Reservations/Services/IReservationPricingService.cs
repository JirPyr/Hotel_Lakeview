namespace HotelLakeview.Application.Reservations.Services;

/// <summary>
/// Rajapinta varauksen hinnan laskentaan.
/// </summary>
public interface IReservationPricingService
{
    /// <summary>
    /// Laskee varauksen kokonaishinnan aikavälille.
    /// </summary>
    /// <param name="basePricePerNight">Huoneen perusyöhinta.</param>
    /// <param name="checkInDate">Sisäänkirjautumispäivä.</param>
    /// <param name="checkOutDate">Uloskirjautumispäivä.</param>
    /// <returns>Varauksen kokonaishinta.</returns>
    decimal CalculateTotalPrice(
        decimal basePricePerNight,
        DateTime checkInDate,
        DateTime checkOutDate);
}