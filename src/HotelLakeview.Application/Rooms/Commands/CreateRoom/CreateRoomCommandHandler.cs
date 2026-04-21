using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Rooms.Dtos;
using HotelLakeview.Domain.Entities;
using MediatR;

namespace HotelLakeview.Application.Rooms.Commands.CreateRoom;

/// <summary>
/// Käsittelijä uuden huoneen luomiseen.
/// </summary>
public sealed class CreateRoomCommandHandler
    : IRequestHandler<CreateRoomCommand, Result<RoomDto>>
{
    private readonly IRoomRepository _roomRepository;

    /// <summary>
    /// Luo käsittelijän huoneen luontia varten.
    /// </summary>
    public CreateRoomCommandHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    /// <inheritdoc />
    public async Task<Result<RoomDto>> Handle(
        CreateRoomCommand request,
        CancellationToken cancellationToken)
    {
        var existingRoom = await _roomRepository.GetByRoomNumberAsync(request.RoomNumber);

        if (existingRoom is not null)
        {
            return Result<RoomDto>.Failure(
                Error.Conflict("Room.RoomNumberAlreadyExists", "Huonenumero on jo käytössä."));
        }

        var room = new Room(
            request.RoomNumber,
            request.RoomType,
            request.MaxGuests,
            request.BasePricePerNight,
            request.Description);

        var createdRoom = await _roomRepository.AddAsync(room);

        var dto = new RoomDto
        {
            Id = createdRoom.Id,
            RoomNumber = createdRoom.RoomNumber,
            RoomType = createdRoom.RoomType,
            MaxGuests = createdRoom.MaxGuests,
            BasePricePerNight = createdRoom.BasePricePerNight,
            Description = createdRoom.Description,
            IsActive = createdRoom.IsActive,
            CreatedAtUtc = createdRoom.CreatedAtUtc,
            UpdatedAtUtc = createdRoom.UpdatedAtUtc
        };

        return Result<RoomDto>.Success(dto);
    }
}