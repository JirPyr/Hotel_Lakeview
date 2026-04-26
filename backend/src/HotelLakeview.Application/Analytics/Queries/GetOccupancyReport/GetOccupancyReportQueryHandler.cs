using HotelLakeview.Application.Analytics.Dtos;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Enums;
using MediatR;

namespace HotelLakeview.Application.Analytics.Queries.GetOccupancyReport;
public sealed class GetOccupancyReportQueryHandler
    : IRequestHandler<GetOccupancyReportQuery, Result<OccupancyReportDto>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationRepository _reservationRepository;

    public GetOccupancyReportQueryHandler(
        IRoomRepository roomRepository,
        IReservationRepository reservationRepository)
    {
        _roomRepository = roomRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<OccupancyReportDto>> Handle(
        GetOccupancyReportQuery request,
        CancellationToken cancellationToken)
    {
        if (request.EndDate <= request.StartDate)
       return Result<OccupancyReportDto>.Failure(
            Error.Validation("InvalidDateRange" , "EndDate must be after StartDate."));

        // 🔹 1. Aktiiviset huoneet
        var activeRooms = await _roomRepository.GetActiveAsync();
        var activeRoomCount = activeRooms.Count;

        // 🔹 2. Päivien määrä
        var totalDays = (request.EndDate.Date - request.StartDate.Date).Days;

        var totalAvailableRoomNights = activeRoomCount * totalDays;

        // 🔹 3. Hae varaukset (voi käyttää olemassa olevaa metodia)
        var reservations = await _reservationRepository.GetActiveByDateRangeAsync(
            request.StartDate,
            request.EndDate);

        // 🔹 4. Suodata vain Confirmed
        var confirmedReservations = reservations
            .Where(r => r.Status == ReservationStatus.Confirmed)
            .ToList();

        int occupiedRoomNights = 0;

        foreach (var reservation in confirmedReservations)
        {
            var overlapStart = reservation.CheckInDate.Date > request.StartDate.Date
                ? reservation.CheckInDate.Date
                : request.StartDate.Date;

            var overlapEnd = reservation.CheckOutDate.Date < request.EndDate.Date
                ? reservation.CheckOutDate.Date
                : request.EndDate.Date;

            var nights = Math.Max(0, (overlapEnd - overlapStart).Days);

            occupiedRoomNights += nights;
        }

        decimal occupancyRate = 0;

        if (totalAvailableRoomNights > 0)
        {
            occupancyRate = (decimal)occupiedRoomNights / totalAvailableRoomNights * 100;
        }

        var result = new OccupancyReportDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ActiveRoomCount = activeRoomCount,
            TotalDays = totalDays,
            TotalAvailableRoomNights = totalAvailableRoomNights,
            OccupiedRoomNights = occupiedRoomNights,
            OccupancyRatePercentage = Math.Round(occupancyRate, 2)
        };

        return Result<OccupancyReportDto>.Success(result);
    }
}