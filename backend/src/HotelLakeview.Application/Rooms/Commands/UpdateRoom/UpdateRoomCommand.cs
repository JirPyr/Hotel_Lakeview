using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Rooms.Dtos;
using HotelLakeview.Domain.Enums;
using MediatR;

namespace HotelLakeview.Application.Rooms.Commands.UpdateRoom;

/// <summary>
/// Komento huoneen päivittämiseen.
/// </summary>
public sealed record UpdateRoomCommand(
    int Id,
    string RoomNumber,
    RoomType RoomType,
    int MaxGuests,
    decimal BasePricePerNight,
    string? Description) : IRequest<Result<RoomDto>>;