namespace HotelLakeview.Application.Customers.DTOs;

/// <summary>
/// Asiakkaan siirtomalli, jota käytetään API-palautuksissa.
/// 
/// DTO erottaa ulospäin palautettavan datan domain-entiteetistä,
/// jolloin Domain pysyy puhtaana eikä sitä vuoda suoraan API-kerrokseen.
/// </summary>
public sealed class CustomerDto
{
    /// <summary>
    /// Asiakkaan yksilöivä tunniste.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Asiakkaan koko nimi.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Asiakkaan sähköpostiosoite.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Asiakkaan puhelinnumero.
    /// </summary>
    public string PhoneNumber { get; init; } = string.Empty;

    /// <summary>
    /// Mahdolliset lisätiedot, kuten allergiat tai erityistoiveet.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Kertoo, onko asiakas aktiivinen.
    /// </summary>
    public bool IsActive { get; init; }
}