using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using MediatR;

namespace HotelLakeview.Application.Rooms.Commands.DeleteRoom;

/// <summary>
/// Käsittelijä huoneen poistamiseen käytöstä.
/// </summary>
public sealed class DeleteRoomCommandHandler
    : IRequestHandler<DeleteRoomCommand, Result>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationRepository _reservationRepository;

    /// <summary>
    /// Luo käsittelijän huoneen poistamista käytöstä varten.
    /// </summary>
    public DeleteRoomCommandHandler(
        IRoomRepository roomRepository,
        IReservationRepository reservationRepository)
    {
        _roomRepository = roomRepository;
        _reservationRepository = reservationRepository;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        DeleteRoomCommand request,
        CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.Id);

        if (room is null)
        {
            return Result.Failure(
                Error.NotFound("Room.NotFound", "Huonetta ei löytynyt."));
        }

        if (!room.IsActive)
        {
            return Result.Failure(
                Error.Conflict("Room.AlreadyInactive", "Huone on jo poistettu käytöstä."));
        }

        var reservations = await _reservationRepository.GetByRoomIdAsync(request.Id);

        var blockingReservation = reservations
        .Where(r => r.IsActive && r.CheckInDate > DateTime.UtcNow)
        .OrderBy(r => r.CheckInDate)
        .FirstOrDefault();

        if (blockingReservation is not null)
        {
            return Result.Failure(
                Error.Conflict(
                    "Room.HasFutureReservations",
                    $"Huonetta ei voi poistaa käytöstä, koska sillä on tuleva aktiivinen varaus. " +
                    $"VarausId: {blockingReservation.Id}, " +
                    $"AsiakasId: {blockingReservation.CustomerId}, " +
                    $"Sisäänkirjautuminen: {blockingReservation.CheckInDate:yyyy-MM-dd}, " +
                    $"Uloskirjautuminen: {blockingReservation.CheckOutDate:yyyy-MM-dd}."));
        }

        room.Deactivate();

        await _roomRepository.UpdateAsync(room);

        return Result.Success();
    }
}