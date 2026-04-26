using HotelLakeview.Application.Reservations.Dtos;
using MediatR;
using HotelLakeview.Application.Common.Results;

namespace HotelLakeview.Application.Reservations.Queries.GetReservationById;

/// <summary>
/// Kysely varauksen hakemiseen tunnisteella.
/// </summary>
public sealed record GetReservationByIdQuery(int Id) : IRequest<Result<ReservationDto>>;