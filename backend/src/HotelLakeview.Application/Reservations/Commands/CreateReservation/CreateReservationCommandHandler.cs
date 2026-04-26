using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Reservations.Dtos;
using HotelLakeview.Application.Reservations.Services;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using MediatR;

namespace HotelLakeview.Application.Reservations.Commands.CreateReservation;

/// <summary>
/// Käsittelijä uuden varauksen luomiseen.
/// </summary>
public sealed class CreateReservationCommandHandler
    : IRequestHandler<CreateReservationCommand, Result<ReservationDto>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationPricingService _pricingService;

    /// <summary>
    /// Luo käsittelijän uuden varauksen luontia varten.
    /// </summary>
    public CreateReservationCommandHandler(
        IReservationRepository reservationRepository,
        ICustomerRepository customerRepository,
        IRoomRepository roomRepository,
        IReservationPricingService pricingService)
    {
        _reservationRepository = reservationRepository;
        _customerRepository = customerRepository;
        _roomRepository = roomRepository;
        _pricingService = pricingService;
    }

    /// <inheritdoc />
    public async Task<Result<ReservationDto>> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

        if (customer is null)
        {
            return Result<ReservationDto>.Failure(
                Error.NotFound("Customer.NotFound", "Asiakasta ei löytynyt."));
        }

        var room = await _roomRepository.GetByIdAsync(request.RoomId);

        if (room is null)
        {
            return Result<ReservationDto>.Failure(
                Error.NotFound("Room.NotFound", "Huonetta ei löytynyt."));
        }

        if(!room.IsActive)
        {
            return Result<ReservationDto>.Failure(
                Error.Conflict("Room.Inactive", "Huone on pois käytöstä."));
        }

        if (request.GuestCount > room.MaxGuests)
        {
            return Result<ReservationDto>.Failure(
                Error.Validation("Reservation.GuestCountExceeded", "Henkilömäärä ylittää huoneen kapasiteetin."));
        }

        var hasOverlap = await _reservationRepository.HasOverlappingReservationAsync(
            request.RoomId,
            request.CheckInDate,
            request.CheckOutDate);

        if (hasOverlap)
        {
            return Result<ReservationDto>.Failure(
                Error.Conflict("Reservation.Overlap", "Huone on jo varattu annetulle aikavälille."));
        }

        var totalPrice = _pricingService.CalculateTotalPrice(
            room.BasePricePerNight,
            request.CheckInDate,
            request.CheckOutDate);

        var reservation = new Reservation(
            request.CustomerId,
            request.RoomId,
            request.CheckInDate,
            request.CheckOutDate,
            request.GuestCount,
            totalPrice,
            request.Notes);

        var createdReservation = await _reservationRepository.AddAsync(reservation);

        var dto = new ReservationDto
        {
            Id = createdReservation.Id,
            CustomerId = createdReservation.CustomerId,
            RoomId = createdReservation.RoomId,
            CheckInDate = createdReservation.CheckInDate,
            CheckOutDate = createdReservation.CheckOutDate,
            GuestCount = createdReservation.GuestCount,
            TotalPrice = createdReservation.TotalPrice,
            Status = createdReservation.Status,
            Notes = createdReservation.Notes,
            CreatedAtUtc = createdReservation.CreatedAtUtc,
            UpdatedAtUtc = createdReservation.UpdatedAtUtc,
            CancelledAtUtc = createdReservation.CancelledAtUtc
        };

        return Result<ReservationDto>.Success(dto);
    }
}