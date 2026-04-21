using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;



namespace HotelLakeview.Application.Interfaces;

/// <summary>
/// Määrittelee huoneiden hakemiseen ja tallentamiseen liittyvät operaatiot.
/// </summary>
public interface  IRoomRepository
{
    Task<Room?> GetByIdAsync(int id);
    Task<Room?> GetByRoomNumberAsync(string roomNumber);
    Task<IReadOnlyCollection<Room>> GetPagedAsync(int page, int pageSize);
    Task<int> CountAsync();
    Task<IReadOnlyCollection<Room>> GetActiveAsync();
    Task<IReadOnlyCollection<Room>> GetByRoomTypeAsync(RoomType roomType);
    Task<Room> AddAsync(Room room);
    Task<Room> UpdateAsync(Room room);
}