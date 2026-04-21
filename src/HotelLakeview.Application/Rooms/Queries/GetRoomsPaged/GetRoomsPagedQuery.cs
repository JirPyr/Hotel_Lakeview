using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Rooms.Dtos;
using MediatR;

namespace HotelLakeview.Application.Rooms.Queries.GetRoomsPaged;

/// <summary>
/// Hakee kaikki huoneet sivutettuna.
/// </summary>
public sealed record GetRoomsPagedQuery(int Page, int PageSize)
    : IRequest<Result<PagedResult<RoomDto>>>;