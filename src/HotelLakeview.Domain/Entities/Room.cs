using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Domain.Entities;

/// <summary>
/// Kuvaa yksittäistä hotellihuonetta.
/// Huone on varattava resurssi, jolla on tyyppi, kapasiteetti ja perushinta.
/// </summary>
public class Room
{
    /// <summary>
    /// EF Corea ja serialisointia varten.
    /// </summary>
    private Room()
    {
    }

    /// <summary>
    /// Luo uuden huoneen.
    /// </summary>
    /// <param name="roomNumber">Huoneen numero tai muu yksilöivä tunniste.</param>
    /// <param name="roomType">Huoneen tyyppi.</param>
    /// <param name="maxGuests">Huoneen maksimihenkilömäärä.</param>
    /// <param name="basePricePerNight">Huoneen perushinta per yö.</param>
    /// <param name="description">Huoneen kuvaus.</param>
    /// <exception cref="ArgumentException">Heitetään, jos syöte on virheellinen.</exception>
    public Room(
        string roomNumber,
        RoomType roomType,
        int maxGuests,
        decimal basePricePerNight,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
        {
            throw new ArgumentException("Room number is required.", nameof(roomNumber));
        }

        if (maxGuests <= 0)
        {
            throw new ArgumentException("Max guests must be greater than zero.", nameof(maxGuests));
        }

        if (basePricePerNight <= 0)
        {
            throw new ArgumentException("Base price per night must be greater than zero.", nameof(basePricePerNight));
        }

        RoomNumber = roomNumber.Trim();
        RoomType = roomType;
        MaxGuests = maxGuests;
        BasePricePerNight = basePricePerNight;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Huoneen yksilöivä tunniste.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Huoneen numero tai muu yksilöivä tunniste.
    /// </summary>
    public string RoomNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Huoneen tyyppi.
    /// </summary>
    public RoomType RoomType { get; private set; }

    /// <summary>
    /// Huoneen suurin sallittu henkilömäärä.
    /// </summary>
    public int MaxGuests { get; private set; }

    /// <summary>
    /// Huoneen perushinta per yö.
    /// </summary>
    public decimal BasePricePerNight { get; private set; }

    /// <summary>
    /// Huoneen kuvaus.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Kertoo, onko huone aktiivisesti käytettävissä.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Huoneen luontiaika UTC-aikana.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Huoneen viimeisin päivitysaika UTC-aikana.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Päivittää huoneen tietoja hallitusti.
    /// </summary>
    /// <param name="roomNumber">Huoneen numero.</param>
    /// <param name="roomType">Huoneen tyyppi.</param>
    /// <param name="maxGuests">Maksimihenkilömäärä.</param>
    /// <param name="basePricePerNight">Perushinta per yö.</param>
    /// <param name="description">Huoneen kuvaus.</param>
    /// <exception cref="ArgumentException">Heitetään, jos syöte on virheellinen.</exception>
    public void UpdateDetails(
        string roomNumber,
        RoomType roomType,
        int maxGuests,
        decimal basePricePerNight,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
        {
            throw new ArgumentException("Room number is required.", nameof(roomNumber));
        }

        if (maxGuests <= 0)
        {
            throw new ArgumentException("Max guests must be greater than zero.", nameof(maxGuests));
        }

        if (basePricePerNight <= 0)
        {
            throw new ArgumentException("Base price per night must be greater than zero.", nameof(basePricePerNight));
        }

        RoomNumber = roomNumber.Trim();
        RoomType = roomType;
        MaxGuests = maxGuests;
        BasePricePerNight = basePricePerNight;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Merkitsee huoneen pois käytöstä.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}