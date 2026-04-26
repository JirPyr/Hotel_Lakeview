namespace HotelLakeview.Infrastructure.Storage;

/// <summary>
/// Sisältää Azure Blob Storage -tallennuksen asetukset.
/// Asetukset luetaan konfiguraatiosta BlobStorage-sektion alta.
/// </summary>
public sealed class BlobStorageOptions
{
    /// <summary>
    /// Konfiguraatiosektion nimi.
    /// </summary>
    public const string SectionName = "BlobStorage";

    /// <summary>
    /// Azure Storage Accountin connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Blob containerin nimi, johon huonekuvat tallennetaan.
    /// </summary>
    public string ContainerName { get; set; } = string.Empty;
}