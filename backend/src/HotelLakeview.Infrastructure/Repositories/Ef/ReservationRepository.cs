using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Repositories.Ef;

/// <summary>
/// EF Core -toteutus varausten hallintaan.
/// </summary>
public sealed class ReservationRepository : IReservationRepository
{
    private readonly HotelLakeviewDbContext _dbContext;

    public ReservationRepository(HotelLakeviewDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _dbContext.Reservations
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IReadOnlyCollection<Reservation>> GetPagedAsync(int page, int pageSize)
    {
        return await _dbContext.Reservations
            .OrderByDescending(r => r.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _dbContext.Reservations.CountAsync();
    }

    public async Task<IReadOnlyCollection<Reservation>> GetByCustomerIdAsync(int customerId)
    {
        return await _dbContext.Reservations
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Reservation>> GetByRoomIdAsync(int roomId)
    {
        return await _dbContext.Reservations
            .Where(r => r.RoomId == roomId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Reservation>> GetActiveByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.Reservations
            .Where(r =>
                r.Status != ReservationStatus.Cancelled &&
                r.CheckInDate < endDate &&
                r.CheckOutDate > startDate)
            .OrderBy(r => r.CheckInDate)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingReservationAsync(
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        int? excludeReservationId = null)
    {
        return await _dbContext.Reservations.AnyAsync(r =>
            r.RoomId == roomId &&
            r.Status != ReservationStatus.Cancelled &&
            (!excludeReservationId.HasValue || r.Id != excludeReservationId.Value) &&
            r.CheckInDate < checkOutDate &&
            r.CheckOutDate > checkInDate);
    }
    public Task<IReadOnlyCollection<Reservation>> GetAllByDateRangeAsync(
    DateTime startDate,
    DateTime endDate)
    {
        var items = _dbContext.Reservations
            .Where(r =>
                r.CheckInDate < endDate &&
                r.CheckOutDate > startDate)
            .OrderBy(r => r.CheckInDate)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<Reservation>)items);
    }

    public async Task<Reservation> AddAsync(Reservation reservation)
    {
        await _dbContext.Reservations.AddAsync(reservation);
        await _dbContext.SaveChangesAsync();
        return reservation;
    }

    public async Task<Reservation> UpdateAsync(Reservation reservation)
    {
        _dbContext.Reservations.Update(reservation);
        await _dbContext.SaveChangesAsync();
        return reservation;
    }
    
}