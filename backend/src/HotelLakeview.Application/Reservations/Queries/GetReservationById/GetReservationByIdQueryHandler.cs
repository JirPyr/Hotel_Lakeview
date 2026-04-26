using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Reservations.Dtos;

using MediatR;

namespace HotelLakeview.Application.Reservations.Queries.GetReservationById;

/// <summary>
/// Käsittelijä varauksen hakemiseen tunnisteella.
/// </summary>
public sealed class GetReservationByIdQueryHandler
    : IRequestHandler<GetReservationByIdQuery, Result<ReservationDto>>
{
    private readonly IReservationRepository _reservationRepository;

    /// <summary>
    /// Luo käsittelijän varauksen hakemista varten.
    /// </summary>
    public GetReservationByIdQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    /// <inheritdoc />
    public async Task<Result<ReservationDto>> Handle(
        GetReservationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.Id);

        if (reservation is null)
        {
            return Result<ReservationDto>.Failure(
                Error.NotFound("Reservation.NotFound", "Varausta ei löytynyt."));
        }

        var dto = new ReservationDto
        {
            Id = reservation.Id,
            CustomerId = reservation.CustomerId,
            RoomId = reservation.RoomId,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            GuestCount = reservation.GuestCount,
            TotalPrice = reservation.TotalPrice,
            Status = reservation.Status,
            Notes = reservation.Notes,
            CreatedAtUtc = reservation.CreatedAtUtc,
            UpdatedAtUtc = reservation.UpdatedAtUtc,
            CancelledAtUtc = reservation.CancelledAtUtc
        };

        return Result<ReservationDto>.Success(dto);
    }
}