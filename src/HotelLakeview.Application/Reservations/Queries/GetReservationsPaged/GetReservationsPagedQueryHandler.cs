using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Reservations.Dtos;
using MediatR;

namespace HotelLakeview.Application.Reservations.Queries.GetReservationsPaged;

/// <summary>
/// Käsittelijä varausten sivutettuun hakuun.
/// </summary>
public sealed class GetReservationsPagedQueryHandler
    : IRequestHandler<GetReservationsPagedQuery, Result<PagedResult<ReservationDto>>>
{
    private readonly IReservationRepository _reservationRepository;

    /// <summary>
    /// Luo käsittelijän varausten listausta varten.
    /// </summary>
    public GetReservationsPagedQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<ReservationDto>>> Handle(
        GetReservationsPagedQuery request,
        CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.GetPagedAsync(request.Page, request.PageSize);
        var totalCount = await _reservationRepository.CountAsync();

        var items = reservations
            .Select(reservation => new ReservationDto
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
            })
            .ToList();

        var pagedResult = new PagedResult<ReservationDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount);

        return Result<PagedResult<ReservationDto>>.Success(pagedResult);
    }
}