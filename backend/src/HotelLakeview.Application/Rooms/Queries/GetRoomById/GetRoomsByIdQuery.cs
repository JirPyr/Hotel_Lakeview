using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Rooms.Dtos;
using MediatR;

namespace HotelLakeview.Application.Rooms.Queries.GetRoomById;

/// <summary>
/// Hakee huoneen tunnisteella.
/// </summary>
public sealed record GetRoomByIdQuery(int Id) : IRequest<Result<RoomDto>>;