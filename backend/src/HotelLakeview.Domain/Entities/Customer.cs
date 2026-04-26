namespace HotelLakeview.Domain.Entities;

/// <summary>
/// Kuvaa hotellin asiakasta.
/// Sisältää asiakkaan perustiedot ja domain-tason käyttäytymisen.
/// </summary>
public class Customer
{
    /// <summary>
    /// EF Corea ja serialisointia varten.
    /// </summary>
    private Customer()
    {
    }

    /// <summary>
    /// Luo uuden asiakkaan.
    /// </summary>
    /// <param name="fullName">Asiakkaan koko nimi.</param>
    /// <param name="email">Asiakkaan sähköposti.</param>
    /// <param name="phoneNumber">Asiakkaan puhelinnumero.</param>
    /// <param name="notes">Lisätiedot tai erityistoiveet.</param>
    /// <exception cref="ArgumentException">Heitetään, jos nimi on tyhjä.</exception>
    public Customer(string fullName, string? email, string? phoneNumber, string? notes)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Customer full name is required.", nameof(fullName));
        }

        FullName = fullName.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Asiakkaan yksilöivä tunniste.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Asiakkaan koko nimi.
    /// </summary>
    public string FullName { get; private set; } = string.Empty;

    /// <summary>
    /// Asiakkaan sähköpostiosoite.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Asiakkaan puhelinnumero.
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// Asiakkaaseen liittyvät lisätiedot, kuten allergiat tai erityistoiveet.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Kertoo, onko asiakas aktiivinen.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Asiakkaan luontiaika UTC-aikana.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Asiakkaan viimeisin päivitysaika UTC-aikana.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Päivittää asiakkaan tietoja hallitusti.
    /// </summary>
    /// <param name="fullName">Asiakkaan koko nimi.</param>
    /// <param name="email">Asiakkaan sähköposti.</param>
    /// <param name="phoneNumber">Asiakkaan puhelinnumero.</param>
    /// <param name="notes">Lisätiedot.</param>
    /// <exception cref="ArgumentException">Heitetään, jos nimi on tyhjä.</exception>
    public void UpdateDetails(string fullName, string? email, string? phoneNumber, string? notes)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Customer full name is required.", nameof(fullName));
        }

        FullName = fullName.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Merkitsee asiakkaan passiiviseksi.
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