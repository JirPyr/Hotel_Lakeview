using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Rooms.Dtos;
using MediatR;

namespace HotelLakeview.Application.Rooms.Queries.GetRoomsPaged;

/// <summary>
/// Käsittelijä kaikkien huoneiden sivutettuun hakuun.
/// </summary>
public sealed class GetRoomsPagedQueryHandler
    : IRequestHandler<GetRoomsPagedQuery, Result<PagedResult<RoomDto>>>
{
    private readonly IRoomRepository _roomRepository;

    /// <summary>
    /// Luo käsittelijän huoneiden sivutettua listausta varten.
    /// </summary>
    public GetRoomsPagedQueryHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<RoomDto>>> Handle(
        GetRoomsPagedQuery request,
        CancellationToken cancellationToken)
    {
        var rooms = await _roomRepository.GetPagedAsync(request.Page, request.PageSize);
        var totalCount = await _roomRepository.CountAsync();

        var items = rooms
            .Select(room => new RoomDto
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
            })
            .ToList();

        var pagedResult = new PagedResult<RoomDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount);

        return Result<PagedResult<RoomDto>>.Success(pagedResult);
    }
}