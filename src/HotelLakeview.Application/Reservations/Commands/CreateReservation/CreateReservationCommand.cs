using HotelLakeview.Application.Reservations.Dtos;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Domain.Enums;
using MediatR;

namespace HotelLakeview.Application.Reservations.Commands.CreateReservation;

/// <summary>
/// Komento uuden varauksen luomiseen.
/// </summary>
public sealed record CreateReservationCommand(
    int CustomerId,
    int RoomId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int GuestCount,
    string? Notes) : IRequest<Result<ReservationDto>>;