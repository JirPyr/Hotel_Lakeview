using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Repositories.Ef;

public sealed class RoomRepository : IRoomRepository
{
    private readonly HotelLakeviewDbContext _dbContext;

    public RoomRepository(HotelLakeviewDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _dbContext.Rooms
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Room?> GetByRoomNumberAsync(string roomNumber)
    {
        return await _dbContext.Rooms
            .FirstOrDefaultAsync(r =>
                r.RoomNumber == roomNumber);
    }

    public async Task<IReadOnlyCollection<Room>> GetActiveAsync()
    {
        return await _dbContext.Rooms
            .Where(r => r.IsActive)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Room>> GetByRoomTypeAsync(RoomType roomType)
    {
        return await _dbContext.Rooms
            .Where(r => r.RoomType == roomType)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Room>> GetPagedAsync(int page, int pageSize)
    {
        return await _dbContext.Rooms
            .OrderBy(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _dbContext.Rooms.CountAsync();
    }

    public async Task<IReadOnlyCollection<Room>> GetAvailableRoomsAsync(
        DateTime checkIn,
        DateTime checkOut,
        int guestCount)
    {
        return await _dbContext.Rooms
            .Where(r =>
                r.IsActive &&
                r.MaxGuests >= guestCount &&
                !_dbContext.Reservations.Any(res =>
                    res.RoomId == r.Id &&
                    res.Status != ReservationStatus.Cancelled &&
                    res.CheckInDate < checkOut &&
                    res.CheckOutDate > checkIn))
            .ToListAsync();
    }

    public async Task<Room> AddAsync(Room room)
    {
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.SaveChangesAsync();
        return room;
    }

    public async Task<Room> UpdateAsync(Room room)
    {
        _dbContext.Rooms.Update(room);
        await _dbContext.SaveChangesAsync();
        return room;
    }
}