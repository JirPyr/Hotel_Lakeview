using HotelLakeview.Application.Analytics.Dtos;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Enums;
using MediatR;

namespace HotelLakeview.Application.Analytics.Queries.GetPopularRoomTypesReport;

/// <summary>
/// Käsittelijä suosituimpien huonetyyppien raportin hakemiseen.
/// </summary>
public sealed class GetPopularRoomTypesReportQueryHandler
    : IRequestHandler<GetPopularRoomTypesReportQuery, Result<PopularRoomTypesReportDto>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;

    /// <summary>
    /// Luo käsittelijän suosituimpien huonetyyppien raporttia varten.
    /// </summary>
    public GetPopularRoomTypesReportQueryHandler(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
    }

    /// <inheritdoc />
    public async Task<Result<PopularRoomTypesReportDto>> Handle(
        GetPopularRoomTypesReportQuery request,
        CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.GetActiveByDateRangeAsync(
            request.StartDate,
            request.EndDate);

        var confirmedReservations = reservations
            .Where(r => r.Status == ReservationStatus.Confirmed)
            .ToList();

        var rooms = await _roomRepository.GetActiveAsync();

        var roomTypeByRoomId = rooms.ToDictionary(r => r.Id, r => r.RoomType);

        var reservationsWithRoomType = confirmedReservations
            .Where(r => roomTypeByRoomId.ContainsKey(r.RoomId))
            .Select(r => new
            {
                RoomType = roomTypeByRoomId[r.RoomId]
            })
            .ToList();

        var totalReservationCount = reservationsWithRoomType.Count;

        var groupedRoomTypes = reservationsWithRoomType
            .GroupBy(x => x.RoomType)
            .Select(group => new PopularRoomTypeDto
            {
                RoomType = group.Key,
                ReservationCount = group.Count(),
                PercentageOfReservations = totalReservationCount == 0
                    ? 0
                    : Math.Round((decimal)group.Count() / totalReservationCount * 100, 2)
            })
            .OrderByDescending(x => x.ReservationCount)
            .ThenBy(x => x.RoomType)
            .ToList();

        var dto = new PopularRoomTypesReportDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalReservationCount = totalReservationCount,
            RoomTypes = groupedRoomTypes
        };

        return Result<PopularRoomTypesReportDto>.Success(dto);
    }
}