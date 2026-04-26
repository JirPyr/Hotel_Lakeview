using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using System.Reflection;

namespace HotelLakeview.Infrastructure.Repositories;

/// <summary>
/// In-memory-toteutus varausten tallennukseen ja hakemiseen.
/// </summary>
public class InMemoryReservationRepository : IReservationRepository
{
    private static readonly List<Reservation> Reservations = new();
    private static int _nextId = 1;

    private static bool _isSeeded = false;

    public InMemoryReservationRepository()
    {
        SeedReservations();
    }

    public Task<Reservation?> GetByIdAsync(int id)
    {
        var reservation = Reservations.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(reservation);
    }

    public Task<IReadOnlyCollection<Reservation>> GetPagedAsync(int page, int pageSize)
    {
        var items = Reservations
            .OrderByDescending(r => r.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<Reservation>)items);
    }

    public Task<int> CountAsync()
    {
        return Task.FromResult(Reservations.Count);
    }

    public Task<IReadOnlyCollection<Reservation>> GetByCustomerIdAsync(int customerId)
    {
        var items = Reservations
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<Reservation>)items);
    }
    public Task<IReadOnlyCollection<Reservation>> GetAllByDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        var items = Reservations
            .Where(r =>
                r.CheckInDate < endDate &&
                r.CheckOutDate > startDate)
            .OrderBy(r => r.CheckInDate)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<Reservation>)items);
    }
    public Task<IReadOnlyCollection<Reservation>> GetByRoomIdAsync(int roomId)
    {
        var items = Reservations
            .Where(r => r.RoomId == roomId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<Reservation>)items);
    }

    public Task<IReadOnlyCollection<Reservation>> GetActiveByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var items = Reservations
            .Where(r =>
                r.Status != ReservationStatus.Cancelled &&
                r.CheckInDate < endDate &&
                r.CheckOutDate > startDate)
            .OrderBy(r => r.CheckInDate)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<Reservation>)items);
    }

    public Task<bool> HasOverlappingReservationAsync(
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        int? excludeReservationId = null)
    {
        var hasOverlap = Reservations.Any(r =>
            r.RoomId == roomId &&
            r.Status != ReservationStatus.Cancelled &&
            (!excludeReservationId.HasValue || r.Id != excludeReservationId.Value) &&
            r.CheckInDate < checkOutDate &&
            r.CheckOutDate > checkInDate);

        return Task.FromResult(hasOverlap);
    }

    public Task<Reservation> AddAsync(Reservation reservation)
    {
        SetEntityId(reservation, _nextId++);
        Reservations.Add(reservation);

        return Task.FromResult(reservation);
    }

    public Task<Reservation> UpdateAsync(Reservation reservation)
    {
        var existingIndex = Reservations.FindIndex(r => r.Id == reservation.Id);

        if (existingIndex >= 0)
        {
            Reservations[existingIndex] = reservation;
        }

        return Task.FromResult(reservation);
    }

    private static void SetEntityId(Reservation reservation, int id)
    {
        var property = typeof(Reservation).GetProperty(nameof(Reservation.Id), BindingFlags.Instance | BindingFlags.Public);
        property?.SetValue(reservation, id);
    }
    private static void SeedReservations()
    {
        if (_isSeeded)
        {
            return;
        }

        var seedReservations = new List<Reservation>
        {
          // 🔹 HUONE 201 (Standard) - useita eri tilanteita
        new Reservation(1, 9, new DateTime(2026, 4, 20), new DateTime(2026, 4, 22), 2, 238m, "Business trip"),
        new Reservation(2, 9, new DateTime(2026, 4, 25), new DateTime(2026, 4, 27), 2, 238m, "Weekend stay"),

        // 🔹 HUONE 202
        new Reservation(3, 10, new DateTime(2026, 4, 21), new DateTime(2026, 4, 23), 2, 238m, "Late arrival"),
        new Reservation(4, 10, new DateTime(2026, 5, 1), new DateTime(2026, 5, 3), 2, 238m, "City visit"),

        // 🔹 HUONE 301 (Superior)
        new Reservation(5, 19, new DateTime(2026, 5, 10), new DateTime(2026, 5, 13), 2, 477m, "Couple getaway"),

        // 🔹 HUONE 401 (Junior Suite)
        new Reservation(6, 25, new DateTime(2026, 6, 5), new DateTime(2026, 6, 8), 3, 854.1m, "Family vacation"),

        // 🔹 HUONE 501 (Suite)
        new Reservation(7, 29, new DateTime(2026, 7, 1), new DateTime(2026, 7, 5), 4, 1658.8m, "Summer holiday"),

        // 🔹 HUONE 502 (Suite)
        new Reservation(8, 30, new DateTime(2026, 12, 22), new DateTime(2026, 12, 26), 2, 1658.8m, "Christmas stay"),

        // 🔹 LYHYT VARAUS (testaa edge-case)
        new Reservation(9, 1, new DateTime(2026, 4, 20), new DateTime(2026, 4, 21), 1, 79m, "One night stay"),

        // 🔹 PERUTTU VARAUS (tärkeä!)
        CreateCancelledReservation(10, 2, new DateTime(2026, 4, 22), new DateTime(2026, 4, 24), 1, 158m)
    };

        foreach (var reservation in seedReservations)
        {
            SetEntityId(reservation, _nextId++);
            Reservations.Add(reservation);
        }

        _isSeeded = true;
    }

    private static Reservation CreateCancelledReservation(
        int customerId,
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        int guestCount,
        decimal totalPrice)
    {
        var reservation = new Reservation(
            customerId,
            roomId,
            checkInDate,
            checkOutDate,
            guestCount,
            totalPrice,
            "Peruttu testivaraus");

        reservation.Cancel();

        return reservation;
    }

}
