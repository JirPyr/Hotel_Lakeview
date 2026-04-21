using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using MediatR;

namespace HotelLakeview.Application.Reservations.Commands.CancelReservation;

/// <summary>
/// Käsittelijä varauksen perumiseen.
/// </summary>
public sealed class CancelReservationCommandHandler
    : IRequestHandler<CancelReservationCommand, Result>
{
    private readonly IReservationRepository _reservationRepository;

    /// <summary>
    /// Luo käsittelijän varauksen perumista varten.
    /// </summary>
    public CancelReservationCommandHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        CancelReservationCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.Id);

        if (reservation is null)
        {
            return Result.Failure(
                Error.NotFound("Reservation.NotFound", "Varausta ei löytynyt."));
        }

        if (reservation.Status == Domain.Enums.ReservationStatus.Cancelled)
        {
            return Result.Failure(
                Error.Conflict("Reservation.AlreadyCancelled", "Varaus on jo peruttu."));
        }

        reservation.Cancel();

        await _reservationRepository.UpdateAsync(reservation);

        return Result.Success();
    }
}