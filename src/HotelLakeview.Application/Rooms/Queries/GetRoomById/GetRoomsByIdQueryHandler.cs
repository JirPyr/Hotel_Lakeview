using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Rooms.Dtos;
using MediatR;

namespace HotelLakeview.Application.Rooms.Queries.GetRoomById;

/// <summary>
/// Käsittelijä huoneen hakemiseen tunnisteella.
/// </summary>
public sealed class GetRoomByIdQueryHandler
    : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
{
    private readonly IRoomRepository _roomRepository;

    /// <summary>
    /// Luo käsittelijän huoneen hakemista varten.
    /// </summary>
    public GetRoomByIdQueryHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    /// <inheritdoc />
    public async Task<Result<RoomDto>> Handle(
        GetRoomByIdQuery request,
        CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.Id);

        if (room is null)
        {
            return Result<RoomDto>.Failure(
                Error.NotFound("Room.NotFound", "Huonetta ei löytynyt."));
        }

        var dto = new RoomDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            RoomType = room.RoomType,
            MaxGuests = room.MaxGuests,
            BasePricePerNight = room.BasePricePerNight,
            Description = room.Description,
            IsActive = room.IsActive,
            CreatedAtUtc = room.CreatedAtUtc,
            UpdatedAtUtc = room.UpdatedAtUtc
        };

        return Result<RoomDto>.Success(dto);
    }
}