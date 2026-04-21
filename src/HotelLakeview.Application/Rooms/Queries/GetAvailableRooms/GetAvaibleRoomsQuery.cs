using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Rooms.Dtos;
using MediatR;

namespace HotelLakeview.Application.Rooms.Queries.GetAvailableRooms;

/// <summary>
/// Hakee vapaat huoneet annetulle aikavälille.
/// </summary>
public class GetAvailableRoomsQuery : IRequest<Result<List<AvailableRoomDto>>>
{
    /// <summary>
    /// Sisäänkirjautumispäivä.
    /// </summary>
    public DateTime CheckInDate { get; set; }

    /// <summary>
    /// Uloskirjautumispäivä.
    /// </summary>
    public DateTime CheckOutDate { get; set; }

    /// <summary>
    /// Haluttu henkilömäärä.
    /// </summary>
    public int GuestCount { get; set; }
}