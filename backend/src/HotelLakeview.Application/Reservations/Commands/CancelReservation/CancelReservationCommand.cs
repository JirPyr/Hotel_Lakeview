using HotelLakeview.Application.Common.Results;
using MediatR;


namespace HotelLakeview.Application.Reservations.Commands.CancelReservation;

/// <summary>
/// Komento varauksen perumiseen.
/// </summary>
public sealed record CancelReservationCommand(int Id) : IRequest<Result>;