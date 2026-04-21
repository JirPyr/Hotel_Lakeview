using FluentAssertions;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Xunit;

namespace HotelLakeview.Domain.Tests.Entities;

/// <summary>
/// Yksikkötestit Reservation-entiteetille.
/// </summary>
public sealed class ReservationTests
{
    /// <summary>
    /// Varmistaa, että Cancel asettaa varauksen tilaksi Cancelled
    /// ja asettaa perumisajan.
    /// </summary>
    [Fact]
    public void Cancel_ShouldSetStatusToCancelled()
    {
        // Arrange
        var reservation = new Reservation(
            customerId: 1,
            roomId: 1,
            checkInDate: DateTime.Today.AddDays(1),
            checkOutDate: DateTime.Today.AddDays(3),
            guestCount: 2,
            totalPrice: 200m,
            notes: "Testivaraus");

        // Act
        reservation.Cancel();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
        reservation.CancelledAtUtc.Should().NotBeNull();
        reservation.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Varmistaa, että peruttua varausta ei voi enää päivittää päivämäärien osalta.
    /// </summary>
    [Fact]
    public void UpdateDates_ShouldThrow_WhenReservationIsCancelled()
    {
        // Arrange
        var reservation = new Reservation(
            customerId: 1,
            roomId: 1,
            checkInDate: DateTime.Today.AddDays(1),
            checkOutDate: DateTime.Today.AddDays(3),
            guestCount: 2,
            totalPrice: 200m,
            notes: "Testivaraus");

        reservation.Cancel();

        // Act
        Action act = () => reservation.UpdateDates(
            DateTime.Today.AddDays(2),
            DateTime.Today.AddDays(4));

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Varmistaa, että peruttua varausta ei voi enää vaihtaa toiseen huoneeseen.
    /// </summary>
    [Fact]
    public void ChangeRoom_ShouldThrow_WhenReservationIsCancelled()
    {
        // Arrange
        var reservation = new Reservation(
            customerId: 1,
            roomId: 1,
            checkInDate: DateTime.Today.AddDays(1),
            checkOutDate: DateTime.Today.AddDays(3),
            guestCount: 2,
            totalPrice: 200m,
            notes: "Testivaraus");

        reservation.Cancel();

        // Act
        Action act = () => reservation.ChangeRoom(2);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Varmistaa, että UpdateGuestCount heittää virheen,
    /// jos henkilömäärä on epäkelpo.
    /// </summary>
    [Fact]
    public void UpdateGuestCount_ShouldThrow_WhenGuestCountIsInvalid()
    {
        // Arrange
        var reservation = new Reservation(
            customerId: 1,
            roomId: 1,
            checkInDate: DateTime.Today.AddDays(1),
            checkOutDate: DateTime.Today.AddDays(3),
            guestCount: 2,
            totalPrice: 200m,
            notes: "Testivaraus");

        // Act
        Action act = () => reservation.UpdateGuestCount(0);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}