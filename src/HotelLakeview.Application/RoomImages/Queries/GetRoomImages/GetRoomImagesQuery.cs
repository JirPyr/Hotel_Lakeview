using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.RoomImages.Dtos;
using MediatR;

namespace HotelLakeview.Application.RoomImages.Queries.GetRoomImages;

/// <summary>
/// Kysely huoneeseen liitettyjen kuvien hakemiseen.
/// </summary>
public sealed record GetRoomImagesQuery(int RoomId)
    : IRequest<Result<IReadOnlyCollection<RoomImageDto>>>;