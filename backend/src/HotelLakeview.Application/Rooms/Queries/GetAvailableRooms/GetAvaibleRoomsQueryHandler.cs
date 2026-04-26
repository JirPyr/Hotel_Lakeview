using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Rooms.Dtos;
using MediatR;

namespace HotelLakeview.Application.Rooms.Queries.GetAvailableRooms;

/// <summary>
/// Käsittelee vapaiden huoneiden haun annetulle aikavälille.
/// </summary>
public class GetAvailableRoomsQueryHandler
    : IRequestHandler<GetAvailableRoomsQuery, Result<List<AvailableRoomDto>>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationRepository _reservationRepository;

    /// <summary>
    /// Luo käsittelijän vapaiden huoneiden hakua varten.
    /// </summary>
    public GetAvailableRoomsQueryHandler(
        IRoomRepository roomRepository,
        IReservationRepository reservationRepository)
    {
        _roomRepository = roomRepository;
        _reservationRepository = reservationRepository;
    }

    /// <summary>
    /// Hakee vapaat huoneet annetulle aikavälille ja henkilömäärälle.
    /// </summary>
    public async Task<Result<List<AvailableRoomDto>>> Handle(
        GetAvailableRoomsQuery request,
        CancellationToken cancellationToken)
    {
        var activeRooms = await _roomRepository.GetActiveAsync();

        var activeReservations = await _reservationRepository.GetActiveByDateRangeAsync(
            request.CheckInDate,
            request.CheckOutDate);

        var reservedRoomIds = activeReservations
            .Select(x => x.RoomId)
            .Distinct()
            .ToHashSet();

        var availableRooms = activeRooms
            .Where(room => room.MaxGuests >= request.GuestCount)
            .Where(room => !reservedRoomIds.Contains(room.Id))
            .OrderBy(room => room.BasePricePerNight)
            .Select(room => new AvailableRoomDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                MaxGuests = room.MaxGuests,
                BasePricePerNight = room.BasePricePerNight,
                Description = room.Description
            })
            .ToList();

        return Result<List<AvailableRoomDto>>.Success(availableRooms);
    }
}