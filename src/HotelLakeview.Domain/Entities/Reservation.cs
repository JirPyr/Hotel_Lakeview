using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Domain.Entities;

/// <summary>
/// Kuvaa hotellin huonevarausta.
/// Sisältää varauksen ajankohdan, asiakkaan, huoneen, henkilömäärän ja hinnan.
/// </summary>
public class Reservation
{
    /// <summary>
    /// EF Corea ja serialisointia varten.
    /// </summary>
    private Reservation()
    {
    }

    /// <summary>
    /// Luo uuden varauksen.
    /// </summary>
    /// <param name="customerId">Asiakkaan tunniste.</param>
    /// <param name="roomId">Huoneen tunniste.</param>
    /// <param name="checkInDate">Sisäänkirjautumispäivä.</param>
    /// <param name="checkOutDate">Uloskirjautumispäivä.</param>
    /// <param name="guestCount">Henkilömäärä.</param>
    /// <param name="totalPrice">Varauksen kokonaishinta.</param>
    /// <param name="notes">Varaukseen liittyvät lisätiedot.</param>
    /// <exception cref="ArgumentException">Heitetään, jos syöte on virheellinen.</exception>
    public Reservation(
        int customerId,
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        int guestCount,
        decimal totalPrice,
        string? notes)
    {
        if (customerId <= 0)
        {
            throw new ArgumentException("Customer id must be greater than zero.", nameof(customerId));
        }

        if (roomId <= 0)
        {
            throw new ArgumentException("Room id must be greater than zero.", nameof(roomId));
        }

        if (checkOutDate <= checkInDate)
        {
            throw new ArgumentException("Check-out date must be later than check-in date.", nameof(checkOutDate));
        }

        if (guestCount <= 0)
        {
            throw new ArgumentException("Guest count must be greater than zero.", nameof(guestCount));
        }

        if (totalPrice < 0)
        {
            throw new ArgumentException("Total price cannot be negative.", nameof(totalPrice));
        }

        CustomerId = customerId;
        RoomId = roomId;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        GuestCount = guestCount;
        TotalPrice = totalPrice;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        Status = ReservationStatus.Confirmed;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Varauksen yksilöivä tunniste.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Asiakkaan tunniste.
    /// </summary>
    public int CustomerId { get; private set; }

    /// <summary>
    /// Huoneen tunniste.
    /// </summary>
    public int RoomId { get; private set; }

    /// <summary>
    /// Sisäänkirjautumispäivä.
    /// </summary>
    public DateTime CheckInDate { get; private set; }

    /// <summary>
    /// Uloskirjautumispäivä.
    /// </summary>
    public DateTime CheckOutDate { get; private set; }

    /// <summary>
    /// Varauksen henkilömäärä.
    /// </summary>
    public int GuestCount { get; private set; }

    /// <summary>
    /// Varauksen laskettu kokonaishinta.
    /// </summary>
    public decimal TotalPrice { get; private set; }

    /// <summary>
    /// Varauksen tila.
    /// </summary>
    public ReservationStatus Status { get; private set; }

    /// <summary>
    /// Varaukseen liittyvät lisätiedot.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Varauksen luontiaika UTC-aikana.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Varauksen viimeisin päivitysaika UTC-aikana.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Varauksen perumisaika UTC-aikana, jos varaus on peruttu.
    /// </summary>
    public DateTime? CancelledAtUtc { get; private set; }

    /// <summary>
    /// Kertoo, onko varaus aktiivinen.
    /// </summary>
    public bool IsActive => Status != ReservationStatus.Cancelled;

    /// <summary>
    /// Päivittää varauksen päivämääriä.
    /// </summary>
    /// <param name="checkInDate">Uusi sisäänkirjautumispäivä.</param>
    /// <param name="checkOutDate">Uusi uloskirjautumispäivä.</param>
    /// <exception cref="InvalidOperationException">Heitetään, jos varaus on peruttu.</exception>
    /// <exception cref="ArgumentException">Heitetään, jos aikaväli on virheellinen.</exception>
    public void UpdateDates(DateTime checkInDate, DateTime checkOutDate)
    {
        EnsureNotCancelled();

        if (checkOutDate <= checkInDate)
        {
            throw new ArgumentException("Check-out date must be later than check-in date.", nameof(checkOutDate));
        }

        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Päivittää henkilömäärän.
    /// </summary>
    /// <param name="guestCount">Uusi henkilömäärä.</param>
    /// <exception cref="InvalidOperationException">Heitetään, jos varaus on peruttu.</exception>
    /// <exception cref="ArgumentException">Heitetään, jos henkilömäärä on virheellinen.</exception>
    public void UpdateGuestCount(int guestCount)
    {
        EnsureNotCancelled();

        if (guestCount <= 0)
        {
            throw new ArgumentException("Guest count must be greater than zero.", nameof(guestCount));
        }

        GuestCount = guestCount;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Päivittää varauksen kokonaishinnan.
    /// </summary>
    /// <param name="totalPrice">Uusi kokonaishinta.</param>
    /// <exception cref="InvalidOperationException">Heitetään, jos varaus on peruttu.</exception>
    /// <exception cref="ArgumentException">Heitetään, jos hinta on virheellinen.</exception>
    public void SetTotalPrice(decimal totalPrice)
    {
        EnsureNotCancelled();

        if (totalPrice < 0)
        {
            throw new ArgumentException("Total price cannot be negative.", nameof(totalPrice));
        }

        TotalPrice = totalPrice;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Päivittää varauksen lisätiedot.
    /// </summary>
    /// <param name="notes">Uudet lisätiedot.</param>
    /// <exception cref="InvalidOperationException">Heitetään, jos varaus on peruttu.</exception>
    public void UpdateNotes(string? notes)
    {
        EnsureNotCancelled();

        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Peruu varauksen.
    /// </summary>
    public void Cancel()
    {
        if (Status == ReservationStatus.Cancelled)
        {
            return;
        }

        Status = ReservationStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Varmistaa, ettei varaus ole peruttu.
    /// </summary>
    /// <exception cref="InvalidOperationException">Heitetään, jos varaus on peruttu.</exception>
    private void EnsureNotCancelled()
    {
        if (Status == ReservationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled reservation cannot be modified.");
        }
    }
    public void ChangeRoom(int roomId)
{

    EnsureNotCancelled();
    if (roomId <= 0)
    {
        throw new ArgumentException("Room id must be greater than zero.", nameof(roomId));
    }

    RoomId = roomId;
    UpdatedAtUtc = DateTime.UtcNow;
}

}