using FluentAssertions;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Reservations.Commands.CancelReservation;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Reservations.Commands;

/// <summary>
/// Yksikkötestit varauksen perumisen käsittelijälle.
/// </summary>
public sealed class CancelReservationCommandHandlerTests
{
    /// <summary>
    /// Varmistaa, että varaus perutaan onnistuneesti,
    /// kun varaus on olemassa ja se on aktiivinen.
    /// 
    /// Testaa, että:
    /// - handler palauttaa onnistuneen tuloksen
    /// - varauksen tila muuttuu Cancelled-tilaan
    /// - perumisaika asetetaan
    /// - repositoryn UpdateAsync kutsutaan kerran
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCancelReservation_WhenReservationExistsAndIsActive()
    {
        // Arrange
        var reservation = new Reservation(
            customerId: 1,
            roomId: 1,
            checkInDate: new DateTime(2026, 5, 10),
            checkOutDate: new DateTime(2026, 5, 12),
            guestCount: 2,
            totalPrice: 200m,
            notes: "Testivaraus");

        var reservationRepositoryMock = new Mock<IReservationRepository>();

        reservationRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(reservation);

        reservationRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Reservation>()))
            .ReturnsAsync((Reservation r) => r);

        var handler = new CancelReservationCommandHandler(
            reservationRepositoryMock.Object);

        var command = new CancelReservationCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        reservation.Status.Should().Be(ReservationStatus.Cancelled);
        reservation.CancelledAtUtc.Should().NotBeNull();

        reservationRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Reservation>()),
            Times.Once);
    }
    /// <summary>
    /// Varmistaa, että handler palauttaa NotFound-virheen,
    /// kun pyydettyä varausta ei löydy järjestelmästä.
    /// 
    /// Testaa, että:
    /// - tulos on epäonnistunut
    /// - virhetyyppi on NotFound
    /// - UpdateAsync-metodia ei kutsuta
    /// </summary>

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenReservationDoesNotExist()
    {
        // Arrange
        var reservationRepositoryMock = new Mock<IReservationRepository>();

        reservationRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Reservation?)null);

        var handler = new CancelReservationCommandHandler(
            reservationRepositoryMock.Object);

        var command = new CancelReservationCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("Reservation.NotFound");

        reservationRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Reservation>()),
            Times.Never);
    }


    /// <summary>
    /// Varmistaa, että handler palauttaa Conflict-virheen,
    /// kun varaus on jo aiemmin peruttu.
    /// 
    /// Testaa, että:
    /// - tulos on epäonnistunut
    /// - virhetyyppi on Conflict
    /// - oikea virhekoodi palautetaan
    /// - repositoryn UpdateAsync-metodia ei kutsuta uudelleen
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenReservationIsAlreadyCancelled()
    {
        // Arrange
        var reservation = new Reservation(
            customerId: 1,
            roomId: 1,
            checkInDate: new DateTime(2026, 5, 10),
            checkOutDate: new DateTime(2026, 5, 12),
            guestCount: 2,
            totalPrice: 200m,
            notes: "Testivaraus");

        reservation.Cancel();

        var reservationRepositoryMock = new Mock<IReservationRepository>();

        reservationRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(reservation);

        var handler = new CancelReservationCommandHandler(
            reservationRepositoryMock.Object);

        var command = new CancelReservationCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("Reservation.AlreadyCancelled");

        reservationRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Reservation>()),
            Times.Never);
    }
}