using FluentAssertions;
using HotelLakeview.Application.Analytics.Queries.GetPopularRoomTypesReport;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Analytics.Queries.GetPopularRoomTypesReport;

/// <summary>
/// Yksikkötestit suosituimpien huonetyyppien raportin käsittelijälle.
/// </summary>
public sealed class GetPopularRoomTypesReportQueryHandlerTests
{
    /// <summary>
    /// Varmistaa, että handler palauttaa huonetyyppien jakauman oikein,
    /// kun confirmed-varauksia on useista huoneista mutta samoista kategorioista.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnPopularRoomTypesWithCorrectDistribution()
    {
        // Arrange
        var roomRepositoryMock = new Mock<IRoomRepository>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();

        var room1 = new Room("101", RoomType.Economy, 1, 79m, "Economy 1");
        var room2 = new Room("102", RoomType.Economy, 1, 79m, "Economy 2");
        var room3 = new Room("201", RoomType.Standard, 2, 119m, "Standard 1");

        SetRoomId(room1, 1);
        SetRoomId(room2, 2);
        SetRoomId(room3, 3);

        var rooms = new List<Room> { room1, room2, room3 };

        var reservations = new List<Reservation>
        {
            new Reservation(
                customerId: 1,
                roomId: 1,
                checkInDate: new DateTime(2026, 4, 20),
                checkOutDate: new DateTime(2026, 4, 22),
                guestCount: 1,
                totalPrice: 158m,
                notes: "Economy varaus 1"),

            new Reservation(
                customerId: 2,
                roomId: 2,
                checkInDate: new DateTime(2026, 4, 23),
                checkOutDate: new DateTime(2026, 4, 25),
                guestCount: 1,
                totalPrice: 158m,
                notes: "Economy varaus 2"),

            new Reservation(
                customerId: 3,
                roomId: 3,
                checkInDate: new DateTime(2026, 4, 24),
                checkOutDate: new DateTime(2026, 4, 26),
                guestCount: 2,
                totalPrice: 238m,
                notes: "Standard varaus")
        };

        reservationRepositoryMock
            .Setup(x => x.GetActiveByDateRangeAsync(
                new DateTime(2026, 4, 1),
                new DateTime(2026, 4, 30)))
            .ReturnsAsync(reservations);

        roomRepositoryMock
            .Setup(x => x.GetActiveAsync())
            .ReturnsAsync(rooms);

        var handler = new GetPopularRoomTypesReportQueryHandler(
            reservationRepositoryMock.Object,
            roomRepositoryMock.Object);

        var query = new GetPopularRoomTypesReportQuery(
            new DateTime(2026, 4, 1),
            new DateTime(2026, 4, 30));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.TotalReservationCount.Should().Be(3);
        result.Value.RoomTypes.Should().HaveCount(2);

        var economy = result.Value.RoomTypes.Single(x => x.RoomType == RoomType.Economy);
        var standard = result.Value.RoomTypes.Single(x => x.RoomType == RoomType.Standard);

        economy.ReservationCount.Should().Be(2);
        economy.PercentageOfReservations.Should().Be(66.67m);

        standard.ReservationCount.Should().Be(1);
        standard.PercentageOfReservations.Should().Be(33.33m);
    }

    private static void SetRoomId(Room room, int id)
    {
        var property = typeof(Room).GetProperty(nameof(Room.Id));
        property?.SetValue(room, id);
    }
}