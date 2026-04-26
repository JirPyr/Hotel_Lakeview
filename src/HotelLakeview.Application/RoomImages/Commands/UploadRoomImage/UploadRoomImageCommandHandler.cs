using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.RoomImages.Dtos;
using HotelLakeview.Domain.Entities;
using MediatR;

namespace HotelLakeview.Application.RoomImages.Commands.UploadRoomImage;

/// <summary>
/// Käsittelee huonekuvan lataamisen.
/// </summary>
public sealed class UploadRoomImageCommandHandler
    : IRequestHandler<UploadRoomImageCommand, Result<RoomImageDto>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomImageRepository _roomImageRepository;
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Luo uuden käsittelijän huonekuvan lataamista varten.
    /// </summary>
    public UploadRoomImageCommandHandler(
        IRoomRepository roomRepository,
        IRoomImageRepository roomImageRepository,
        IFileStorageService fileStorageService)
    {
        _roomRepository = roomRepository;
        _roomImageRepository = roomImageRepository;
        _fileStorageService = fileStorageService;
    }

    /// <inheritdoc />
    public async Task<Result<RoomImageDto>> Handle(
        UploadRoomImageCommand request,
        CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId);

        if (room is null)
        {
            return Result<RoomImageDto>.Failure(
                Error.NotFound("Room.NotFound", "Huonetta ei löytynyt."));
        }

        if (!room.IsActive)
        {
            return Result<RoomImageDto>.Failure(
                Error.Conflict("Room.Inactive", "Passiiviselle huoneelle ei voi lisätä kuvaa."));
        }
        if (request.IsPrimary)
    {
        var existingImages = await _roomImageRepository.GetByRoomIdAsync(request.RoomId);

        foreach (var existingImage in existingImages.Where(image => image.IsPrimary))
        {
            existingImage.SetPrimary(false);
            await _roomImageRepository.UpdateAsync(existingImage);
        }
    }

        var filePathOrUrl = await _fileStorageService.UploadAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            cancellationToken);

        var roomImage = new RoomImage(
            request.RoomId,
            request.FileName,
            filePathOrUrl,
            request.ContentType,
            request.SortOrder,
            request.IsPrimary);

        var savedImage = await _roomImageRepository.AddAsync(roomImage);

        var dto = new RoomImageDto
        {
            Id = savedImage.Id,
            RoomId = savedImage.RoomId,
            FileName = savedImage.FileName,
            FilePathOrUrl = savedImage.FilePathOrUrl,
            ContentType = savedImage.ContentType,
            SortOrder = savedImage.SortOrder,
            IsPrimary = savedImage.IsPrimary,
            UploadedAtUtc = savedImage.UploadedAtUtc
        };

        return Result<RoomImageDto>.Success(dto);
    }
}