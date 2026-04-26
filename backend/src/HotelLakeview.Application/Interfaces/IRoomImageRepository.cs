using HotelLakeview.Domain.Entities;

namespace HotelLakeview.Application.Interfaces;

/// <summary>
/// Määrittelee huonekuvien hakemiseen ja tallentamiseen liittyvät operaatiot.
/// </summary>
public interface IRoomImageRepository
{
    Task<RoomImage?> GetByIdAsync(int id);
    Task<IReadOnlyCollection<RoomImage>> GetByRoomIdAsync(int roomId);
    Task<RoomImage> AddAsync(RoomImage roomImage);
    Task<RoomImage> UpdateAsync(RoomImage roomImage);
    Task DeleteAsync(int id);
}