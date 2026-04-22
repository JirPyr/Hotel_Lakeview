using FluentAssertions;
using HotelLakeview.Application.Analytics.Queries.GetOccupancyReport;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Analytics.Queries.GetOccupancyReport;

/// <summary>
/// Yksikkötestit käyttöasteraportin käsittelijälle.
/// </summary>
public sealed class GetOccupancyReportQueryHandlerTests
{
    /// <summary>
    /// Varmistaa, että handler laskee käyttöasteen oikein,
    /// kun aikavälillä on aktiivisia huoneita ja confirmed-varauksia.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCalculateOccupancyRateCorrectly()
    {
        // Arrange
        var roomRepositoryMock = new Mock<IRoomRepository>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();

        var rooms = new List<Room>
        {
            new Room("101", RoomType.Economy, 1, 79m, "Huone 101"),
            new Room("102", RoomType.Economy, 1, 79m, "Huone 102"),
            new Room("103", RoomType.Economy, 1, 79m, "Huone 103")
        };

        var reservations = new List<Reservation>
        {
            new Reservation(
                customerId: 1,
                roomId: 1,
                checkInDate: new DateTime(2026, 4, 20),
                checkOutDate: new DateTime(2026, 4, 22),
                guestCount: 1,
                totalPrice: 158m,
                notes: "Varaus 1"),

            new Reservation(
                customerId: 2,
                roomId: 2,
                checkInDate: new DateTime(2026, 4, 21),
                checkOutDate: new DateTime(2026, 4, 23),
                guestCount: 1,
                totalPrice: 158m,
                notes: "Varaus 2")
        };

        roomRepositoryMock
            .Setup(x => x.GetActiveAsync())
            .ReturnsAsync(rooms);

        reservationRepositoryMock
            .Setup(x => x.GetActiveByDateRangeAsync(
                new DateTime(2026, 4, 20),
                new DateTime(2026, 4, 25)))
            .ReturnsAsync(reservations);

        var handler = new GetOccupancyReportQueryHandler(
            roomRepositoryMock.Object,
            reservationRepositoryMock.Object);

        var query = new GetOccupancyReportQuery(
            new DateTime(2026, 4, 20),
            new DateTime(2026, 4, 25));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.StartDate.Should().Be(new DateTime(2026, 4, 20));
        result.Value.EndDate.Should().Be(new DateTime(2026, 4, 25));

        result.Value.ActiveRoomCount.Should().Be(3);
        result.Value.OccupiedRoomNights.Should().Be(4);
        result.Value.OccupancyRatePercentage.Should().Be(26.67m);
    }
}