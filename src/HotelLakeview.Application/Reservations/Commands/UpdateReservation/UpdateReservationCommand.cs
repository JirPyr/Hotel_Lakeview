using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Reservations.Dtos;
using MediatR;

namespace HotelLakeview.Application.Reservations.Commands.UpdateReservation;

/// <summary>
/// Komento varauksen päivittämiseksi.
/// </summary>
/// <param name="Id">Päivitettävän varauksen tunniste.</param>
/// <param name="RoomId">Huoneen tunniste.</param>
/// <param name="CustomerId">Asiakkaan tunniste.</param>
/// <param name="CheckInDate">Saapumispäivä.</param>
/// <param name="CheckOutDate">Lähtöpäivä.</param>
/// <param name="GuestCount">Vieraiden määrä.</param>
public sealed record UpdateReservationCommand(
    int Id,
    int RoomId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int GuestCount,
    string? Notes) : IRequest<Result<ReservationDto>>;
