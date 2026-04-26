using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using MediatR;

namespace HotelLakeview.Application.RoomImages.Commands.DeleteRoomImage;

/// <summary>
/// Käsittelee huonekuvan poistamisen.
/// </summary>
public sealed class DeleteRoomImageCommandHandler
    : IRequestHandler<DeleteRoomImageCommand, Result>
{
    private readonly IRoomImageRepository _roomImageRepository;
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Luo uuden käsittelijän huonekuvan poistamista varten.
    /// </summary>
    public DeleteRoomImageCommandHandler(
        IRoomImageRepository roomImageRepository,
        IFileStorageService fileStorageService)
    {
        _roomImageRepository = roomImageRepository;
        _fileStorageService = fileStorageService;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        DeleteRoomImageCommand request,
        CancellationToken cancellationToken)
    {
        var roomImage = await _roomImageRepository.GetByIdAsync(request.ImageId);

        if (roomImage is null)
        {
            return Result.Failure(
                Error.NotFound("RoomImage.NotFound", "Huonekuvaa ei löytynyt."));
        }

        await _fileStorageService.DeleteAsync(
            roomImage.FilePathOrUrl,
            cancellationToken);

        await _roomImageRepository.DeleteAsync(roomImage.Id);

        return Result.Success();
    }
}