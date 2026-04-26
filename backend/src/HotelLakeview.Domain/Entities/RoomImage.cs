namespace HotelLakeview.Domain.Entities;

/// <summary>
/// Kuvaa huoneeseen liitetyn kuvan metatietoa.
/// </summary>
public class RoomImage
{
    private RoomImage()
    {
    }

    public RoomImage(
        int roomId,
        string fileName,
        string filePathOrUrl,
        string contentType,
        int sortOrder,
        bool isPrimary)
    {
        if (roomId <= 0)
        {
            throw new ArgumentException("Room id must be greater than zero.", nameof(roomId));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required.", nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(filePathOrUrl))
        {
            throw new ArgumentException("File path or URL is required.", nameof(filePathOrUrl));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException("Content type is required.", nameof(contentType));
        }

        RoomId = roomId;
        FileName = fileName.Trim();
        FilePathOrUrl = filePathOrUrl.Trim();
        ContentType = contentType.Trim();
        SortOrder = sortOrder;
        IsPrimary = isPrimary;
        UploadedAtUtc = DateTime.UtcNow;
        
    }

    public int Id { get; private set; }
    public int RoomId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePathOrUrl { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public bool IsPrimary { get; private set; }
    public DateTime UploadedAtUtc { get; private set; }
    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }
}