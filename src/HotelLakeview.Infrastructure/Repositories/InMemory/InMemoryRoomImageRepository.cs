using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using System.Reflection;

namespace HotelLakeview.Infrastructure.Repositories;

/// <summary>
/// In-memory-toteutus huonekuvien tallennukseen ja hakemiseen.
/// </summary>
public class InMemoryRoomImageRepository : IRoomImageRepository
{
    private static readonly List<RoomImage> RoomImages = new();
    private static int _nextId = 1;

    public Task<RoomImage?> GetByIdAsync(int id)
    {
        var image = RoomImages.FirstOrDefault(i => i.Id == id);
        return Task.FromResult(image);
    }

    public Task<IReadOnlyCollection<RoomImage>> GetByRoomIdAsync(int roomId)
    {
        var items = RoomImages
            .Where(i => i.RoomId == roomId)
            .OrderBy(i => i.SortOrder)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<RoomImage>)items);
    }
 public Task<RoomImage> UpdateAsync(RoomImage roomImage)
    {   
    var existingIndex = RoomImages.FindIndex(i => i.Id == roomImage.Id);

    if (existingIndex >= 0)
    {
        RoomImages[existingIndex] = roomImage;
    }

    return Task.FromResult(roomImage);
}

    public Task<RoomImage> AddAsync(RoomImage roomImage)
    {
        SetEntityId(roomImage, _nextId++);
        RoomImages.Add(roomImage);

        return Task.FromResult(roomImage);
    }

    public Task DeleteAsync(int id)
    {
        var image = RoomImages.FirstOrDefault(i => i.Id == id);

        if (image is not null)
        {
            RoomImages.Remove(image);
        }

        return Task.CompletedTask;
    }

    private static void SetEntityId(RoomImage roomImage, int id)
    {
        var property = typeof(RoomImage).GetProperty(nameof(RoomImage.Id), BindingFlags.Instance | BindingFlags.Public);
        property?.SetValue(roomImage, id);
    }
}