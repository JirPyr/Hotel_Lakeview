using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Reservations.Dtos;
using HotelLakeview.Application.Reservations.Services;
using MediatR;

namespace HotelLakeview.Application.Reservations.Commands.UpdateReservation;

/// <summary>
/// Käsittelijä varauksen päivittämiseen.
/// </summary>
public sealed class UpdateReservationCommandHandler
    : IRequestHandler<UpdateReservationCommand, Result<ReservationDto>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationPricingService _pricingService;

    public UpdateReservationCommandHandler(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        IReservationPricingService pricingService)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _pricingService = pricingService;
    }

    public async Task<Result<ReservationDto>> Handle(
        UpdateReservationCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.Id);

        if (reservation is null)
        {
            return Result<ReservationDto>.Failure(
                Error.NotFound("Reservation.NotFound", "Varausta ei löytynyt."));
        }

        if (!reservation.IsActive)
        {
            return Result<ReservationDto>.Failure(
                Error.Conflict(
                    "Reservation.CancelledCannotBeModified",
                    "Peruttua varausta ei voi muokata."));
        }

        var room = await _roomRepository.GetByIdAsync(request.RoomId);

        if (room is null)
        {
            return Result<ReservationDto>.Failure(
                Error.NotFound("Room.NotFound", "Huonetta ei löytynyt."));
        }

        if (!room.IsActive)
        {
            return Result<ReservationDto>.Failure(
                Error.Conflict(
                    "Room.Inactive",
                    "Huone asetettu pois käytöstä."));
        }

        if (request.GuestCount > room.MaxGuests)
        {
            return Result<ReservationDto>.Failure(
                Error.Validation(
                    "Reservation.GuestCountExceeded",
                    "Henkilömäärä ylittää huoneen kapasiteetin."));
        }

        var hasOverlap = await _reservationRepository.HasOverlappingReservationAsync(
            request.RoomId,
            request.CheckInDate,
            request.CheckOutDate,
            request.Id);

        if (hasOverlap)
        {
            return Result<ReservationDto>.Failure(
                Error.Conflict(
                    "Reservation.Overlap",
                    "Huone on jo varattu annetulle aikavälille."));
        }

        reservation.UpdateDates(request.CheckInDate, request.CheckOutDate);
        reservation.UpdateGuestCount(request.GuestCount);

        if (reservation.RoomId != request.RoomId)
        {
            reservation.ChangeRoom(request.RoomId);
        }

        reservation.UpdateNotes(request.Notes);

        var totalPrice = _pricingService.CalculateTotalPrice(
            room.BasePricePerNight,
            request.CheckInDate,
            request.CheckOutDate);

        reservation.SetTotalPrice(totalPrice);

        var updatedReservation = await _reservationRepository.UpdateAsync(reservation);

        var dto = new ReservationDto
        {
            Id = updatedReservation.Id,
            CustomerId = updatedReservation.CustomerId,
            RoomId = updatedReservation.RoomId,
            CheckInDate = updatedReservation.CheckInDate,
            CheckOutDate = updatedReservation.CheckOutDate,
            GuestCount = updatedReservation.GuestCount,
            TotalPrice = updatedReservation.TotalPrice,
            Status = updatedReservation.Status,
            Notes = updatedReservation.Notes,
            CreatedAtUtc = updatedReservation.CreatedAtUtc,
            UpdatedAtUtc = updatedReservation.UpdatedAtUtc,
            CancelledAtUtc = updatedReservation.CancelledAtUtc
        };

        return Result<ReservationDto>.Success(dto);
    }
}