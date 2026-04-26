namespace HotelLakeview.Application.Interfaces;

/// <summary>
/// Määrittelee tiedostojen tallennukseen käytettävän rajapinnan.
/// Application-kerros ei tiedä, tallennetaanko tiedosto Azureen,
/// paikalliselle levylle vai muuhun ulkoiseen palveluun.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Tallentaa tiedoston ulkoiseen tiedostovarastoon.
    /// </summary>
    /// <param name="fileStream">Tallennettavan tiedoston sisältö streaminä.</param>
    /// <param name="fileName">Alkuperäinen tiedostonimi.</param>
    /// <param name="contentType">Tiedoston MIME-tyyppi.</param>
    /// <param name="cancellationToken">Peruutustunniste.</param>
    /// <returns>Tallennetun tiedoston URL tai polku.</returns>
    Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);

    /// <summary>
    /// Poistaa tiedoston ulkoisesta tiedostovarastosta.
    /// </summary>
    /// <param name="filePathOrUrl">Poistettavan tiedoston URL tai polku.</param>
    /// <param name="cancellationToken">Peruutustunniste.</param>
    Task DeleteAsync(
        string filePathOrUrl,
        CancellationToken cancellationToken);
}