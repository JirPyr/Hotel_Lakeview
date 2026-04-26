using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HotelLakeview.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace HotelLakeview.Infrastructure.Storage;

/// <summary>
/// Azure Blob Storage -toteutus tiedostojen tallennukseen.
/// </summary>
public sealed class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobStorageOptions _options;

    /// <summary>
    /// Luo uuden AzureBlobStorageService-instanssin.
    /// </summary>
    public AzureBlobStorageService(IOptions<BlobStorageOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        // 1. Luodaan BlobServiceClient (yhteys Azureen)
        var blobServiceClient = new BlobServiceClient(_options.ConnectionString);

        // 2. Haetaan container
        var containerClient = blobServiceClient.GetBlobContainerClient(_options.ContainerName);

        // 3. Varmistetaan että container on olemassa
        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.Blob,
            cancellationToken: cancellationToken);

        // 4. Luodaan uniikki tiedostonimi (estää overwrite-tilanteet)
        var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";

        // 5. Luodaan BlobClient tiedostolle
        var blobClient = containerClient.GetBlobClient(uniqueFileName);

        // 6. Määritetään content type (tärkeä selaimelle!)
        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = contentType
        };

        // 7. Upload
        await blobClient.UploadAsync(
            fileStream,
            new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            },
            cancellationToken);

        // 8. Palautetaan URL
        return blobClient.Uri.ToString();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        string filePathOrUrl,
        CancellationToken cancellationToken)
    {
        // 1. Luodaan client
        var blobServiceClient = new BlobServiceClient(_options.ConnectionString);

        var containerClient = blobServiceClient.GetBlobContainerClient(_options.ContainerName);

        // 2. Parsitaan blobin nimi URL:stä
        var uri = new Uri(filePathOrUrl);
        var blobName = Path.GetFileName(uri.LocalPath);

        var blobClient = containerClient.GetBlobClient(blobName);

        // 3. Poistetaan jos löytyy
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}