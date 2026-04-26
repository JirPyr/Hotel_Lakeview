using FluentAssertions;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Xunit;

namespace HotelLakeview.Domain.Tests.Entities;

/// <summary>
/// Yksikkötestit Room-entiteetille.
/// </summary>
public sealed class RoomTests
{
    /// <summary>
    /// Varmistaa, että Deactivate asettaa huoneen inaktiiviseksi.
    /// </summary>
    [Fact]
    public void Deactivate_ShouldSetRoomInactive()
    {
        // Arrange
        var room = new Room(
            roomNumber: "101",
            roomType: RoomType.Standard,
            maxGuests: 2,
            basePricePerNight: 100m,
            description: "Testihuone");

        // Act
        room.Deactivate();

        // Assert
        room.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Varmistaa, että UpdateDetails päivittää huoneen tiedot oikein.
    /// </summary>
    [Fact]
    public void UpdateDetails_ShouldUpdateProperties()
    {
        // Arrange
        var room = new Room(
            roomNumber: "101",
            roomType: RoomType.Standard,
            maxGuests: 2,
            basePricePerNight: 100m,
            description: "Testihuone");

        // Act
        room.UpdateDetails(
            roomNumber: "201",
            roomType: RoomType.Superior,
            maxGuests: 3,
            basePricePerNight: 150m,
            description: "Päivitetty huone");

        // Assert
        room.RoomNumber.Should().Be("201");
        room.RoomType.Should().Be(RoomType.Superior);
        room.MaxGuests.Should().Be(3);
        room.BasePricePerNight.Should().Be(150m);
        room.Description.Should().Be("Päivitetty huone");
    }
}