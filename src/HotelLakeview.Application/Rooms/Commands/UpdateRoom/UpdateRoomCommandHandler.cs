using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Rooms.Dtos;
using MediatR;

namespace HotelLakeview.Application.Rooms.Commands.UpdateRoom;

/// <summary>
/// Käsittelijä huoneen päivittämiseen.
/// </summary>
public sealed class UpdateRoomCommandHandler
    : IRequestHandler<UpdateRoomCommand, Result<RoomDto>>
{
    private readonly IRoomRepository _roomRepository;

    /// <summary>
    /// Luo käsittelijän huoneen päivittämistä varten.
    /// </summary>
    public UpdateRoomCommandHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    /// <inheritdoc />
    public async Task<Result<RoomDto>> Handle(
        UpdateRoomCommand request,
        CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.Id);

        if (room is null)
        {
            return Result<RoomDto>.Failure(
                Error.NotFound("Room.NotFound", "Huonetta ei löytynyt."));
        }

        var existingRoomWithSameNumber = await _roomRepository.GetByRoomNumberAsync(request.RoomNumber);

        if (existingRoomWithSameNumber is not null && existingRoomWithSameNumber.Id != request.Id)
        {
            return Result<RoomDto>.Failure(
                Error.Conflict("Room.RoomNumberAlreadyExists", "Huonenumero on jo käytössä."));
        }

        room.UpdateDetails(
            request.RoomNumber,
            request.RoomType,
            request.MaxGuests,
            request.BasePricePerNight,
            request.Description);

        var updatedRoom = await _roomRepository.UpdateAsync(room);

        var dto = new RoomDto
        {
            Id = updatedRoom.Id,
            RoomNumber = updatedRoom.RoomNumber,
            RoomType = updatedRoom.RoomType,
            MaxGuests = updatedRoom.MaxGuests,
            BasePricePerNight = updatedRoom.BasePricePerNight,
            Description = updatedRoom.Description,
            IsActive = updatedRoom.IsActive,
            CreatedAtUtc = updatedRoom.CreatedAtUtc,
            UpdatedAtUtc = updatedRoom.UpdatedAtUtc
        };

        return Result<RoomDto>.Success(dto);
    }
}