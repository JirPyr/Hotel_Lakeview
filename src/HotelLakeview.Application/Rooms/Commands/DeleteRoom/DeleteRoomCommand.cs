using HotelLakeview.Application.Common.Results;
using MediatR;

namespace HotelLakeview.Application.Rooms.Commands.DeleteRoom;

/// <summary>
/// Komento huoneen poistamiseen käytöstä.
/// </summary>
public sealed record DeleteRoomCommand(int Id) : IRequest<Result>;