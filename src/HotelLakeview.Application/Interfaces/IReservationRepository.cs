using HotelLakeview.Domain.Entities;

namespace HotelLakeview.Application.Interfaces;

/// <summary>
/// Määrittelee varausten hakemiseen ja tallentamiseen liittyvät operaatiot.
/// </summary>
public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(int id);
    Task<IReadOnlyCollection<Reservation>> GetPagedAsync(int page, int pageSize);
    Task<int> CountAsync();
    Task<IReadOnlyCollection<Reservation>> GetByCustomerIdAsync(int customerId);
    Task<IReadOnlyCollection<Reservation>> GetByRoomIdAsync(int roomId);
    Task<IReadOnlyCollection<Reservation>> GetActiveByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<bool> HasOverlappingReservationAsync(
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        int? excludeReservationId = null);

    Task<Reservation> AddAsync(Reservation reservation);
    Task<Reservation> UpdateAsync(Reservation reservation);
}