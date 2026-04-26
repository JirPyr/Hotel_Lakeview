using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.RoomImages.Dtos;
using MediatR;

namespace HotelLakeview.Application.RoomImages.Commands.UploadRoomImage;

/// <summary>
/// Komento huonekuvan lataamiseen.
/// </summary>
public sealed record UploadRoomImageCommand(
    int RoomId,
    string FileName,
    string ContentType,
    Stream FileStream,
    int SortOrder,
    bool IsPrimary) : IRequest<Result<RoomImageDto>>;