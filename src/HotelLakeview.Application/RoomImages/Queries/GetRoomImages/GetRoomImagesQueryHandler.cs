using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.RoomImages.Dtos;
using MediatR;

namespace HotelLakeview.Application.RoomImages.Queries.GetRoomImages;

/// <summary>
/// Käsittelee huonekuvien hakemisen.
/// </summary>
public sealed class GetRoomImagesQueryHandler
    : IRequestHandler<GetRoomImagesQuery, Result<IReadOnlyCollection<RoomImageDto>>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomImageRepository _roomImageRepository;

    /// <summary>
    /// Luo uuden käsittelijän huonekuvien hakemista varten.
    /// </summary>
    public GetRoomImagesQueryHandler(
        IRoomRepository roomRepository,
        IRoomImageRepository roomImageRepository)
    {
        _roomRepository = roomRepository;
        _roomImageRepository = roomImageRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyCollection<RoomImageDto>>> Handle(
        GetRoomImagesQuery request,
        CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId);

        if (room is null)
        {
            return Result<IReadOnlyCollection<RoomImageDto>>.Failure(
                Error.NotFound("Room.NotFound", "Huonetta ei löytynyt."));
        }

        var images = await _roomImageRepository.GetByRoomIdAsync(request.RoomId);

        var dtoList = images
            .Select(image => new RoomImageDto
            {
                Id = image.Id,
                RoomId = image.RoomId,
                FileName = image.FileName,
                FilePathOrUrl = image.FilePathOrUrl,
                ContentType = image.ContentType,
                SortOrder = image.SortOrder,
                IsPrimary = image.IsPrimary,
                UploadedAtUtc = image.UploadedAtUtc
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyCollection<RoomImageDto>>.Success(dtoList);
    }
}