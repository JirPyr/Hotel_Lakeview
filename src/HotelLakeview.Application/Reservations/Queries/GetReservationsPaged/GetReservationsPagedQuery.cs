using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Reservations.Dtos;
using MediatR;

namespace HotelLakeview.Application.Reservations.Queries.GetReservationsPaged;

/// <summary>
/// Hakee varaukset sivutettuna.
/// </summary>
public sealed record GetReservationsPagedQuery(int Page, int PageSize)
    : IRequest<Result<PagedResult<ReservationDto>>>;