using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Rooms.Dtos;
using HotelLakeview.Domain.Enums;
using MediatR;

namespace HotelLakeview.Application.Rooms.Commands.CreateRoom;

/// <summary>
/// Komento uuden huoneen luomiseen.
/// </summary>
public sealed record CreateRoomCommand(
    string RoomNumber,
    RoomType RoomType,
    int MaxGuests,
    decimal BasePricePerNight,
    string? Description) : IRequest<Result<RoomDto>>;