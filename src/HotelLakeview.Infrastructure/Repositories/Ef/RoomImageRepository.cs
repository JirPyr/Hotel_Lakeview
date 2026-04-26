using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Repositories.Ef;

/// <summary>
/// EF Core -toteutus huonekuvien hallintaan.
/// </summary>
public sealed class RoomImageRepository : IRoomImageRepository
{
    private readonly HotelLakeviewDbContext _dbContext;

    public RoomImageRepository(HotelLakeviewDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoomImage?> GetByIdAsync(int id)
    {
        return await _dbContext.RoomImages
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IReadOnlyCollection<RoomImage>> GetByRoomIdAsync(int roomId)
    {
        return await _dbContext.RoomImages
            .Where(i => i.RoomId == roomId)
            .OrderBy(i => i.SortOrder)
            .ToListAsync();
    }


    public async Task<RoomImage> AddAsync(RoomImage roomImage)
    {
        await _dbContext.RoomImages.AddAsync(roomImage);
        await _dbContext.SaveChangesAsync();
        return roomImage;
    }

    public async Task DeleteAsync(int id)
    {
        var image = await _dbContext.RoomImages.FirstOrDefaultAsync(i => i.Id == id);

        if (image is null)
        {
            return;
        }

        _dbContext.RoomImages.Remove(image);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<RoomImage> UpdateAsync(RoomImage roomImage)
    {
        _dbContext.RoomImages.Update(roomImage);
        await _dbContext.SaveChangesAsync();

        return roomImage;
    }
}