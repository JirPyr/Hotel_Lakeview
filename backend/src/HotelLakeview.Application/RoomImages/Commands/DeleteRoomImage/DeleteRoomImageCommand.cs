using HotelLakeview.Application.Common.Results;
using MediatR;

namespace HotelLakeview.Application.RoomImages.Commands.DeleteRoomImage;

/// <summary>
/// Komento huonekuvan poistamiseen.
/// </summary>
public sealed record DeleteRoomImageCommand(int ImageId) : IRequest<Result>;