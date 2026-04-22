using FluentAssertions;
using HotelLakeview.Application.Analytics.Queries.GetRevenueReport;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Analytics.Queries.GetRevenueReport;

/// <summary>
/// Yksikkötestit liikevaihtoraportin käsittelijälle.
/// </summary>
public sealed class GetRevenueReportQueryHandlerTests
{
    /// <summary>
    /// Varmistaa, että handler jakaa liikevaihdon kuukausille oikein,
    /// kun varaus ylittää kuukauden vaihteen.
    ///
    /// Testaa, että:
    /// - handler palauttaa onnistuneen tuloksen
    /// - liikevaihto jaetaan yökohtaisesti eri kuukausille
    /// - molemmille kuukausille syntyy oma raporttirivi
    /// - kokonaisliikevaihto lasketaan oikein
    /// </summary>
    [Fact]
    public async Task Handle_ShouldDistributeRevenueAcrossMonthsCorrectly()
    {
        // Arrange
        var reservationRepositoryMock = new Mock<IReservationRepository>();

        var reservations = new List<Reservation>
        {
            new Reservation(
                customerId: 1,
                roomId: 1,
                checkInDate: new DateTime(2026, 4, 30),
                checkOutDate: new DateTime(2026, 5, 2),
                guestCount: 1,
                totalPrice: 200m,
                notes: "Kuukauden vaihteen ylittävä varaus")
        };

        reservationRepositoryMock
            .Setup(x => x.GetActiveByDateRangeAsync(
                new DateTime(2026, 4, 1),
                new DateTime(2026, 5, 31)))
            .ReturnsAsync(reservations);

        var handler = new GetRevenueReportQueryHandler(
            reservationRepositoryMock.Object);

        var query = new GetRevenueReportQuery(
            new DateTime(2026, 4, 1),
            new DateTime(2026, 5, 31));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.StartDate.Should().Be(new DateTime(2026, 4, 1));
        result.Value.EndDate.Should().Be(new DateTime(2026, 5, 31));

        result.Value.Months.Should().HaveCount(2);

        var april = result.Value.Months.Single(x => x.Year == 2026 && x.Month == 4);
        var may = result.Value.Months.Single(x => x.Year == 2026 && x.Month == 5);

        // Varaus on 2 yötä, kokonaishinta 200 € => 100 € / yö
        // 30.4. yö kuuluu huhtikuulle
        // 1.5. yö kuuluu toukokuulle
        april.Revenue.Should().Be(100m);
        may.Revenue.Should().Be(100m);

        result.Value.TotalRevenue.Should().Be(200m);
    }
}