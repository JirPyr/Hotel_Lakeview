using FluentAssertions;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Rooms.Commands.DeleteRoom;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Rooms.Commands;

/// <summary>
/// Yksikkötestit huoneen deaktivoinnin käsittelijälle.
/// </summary>
public sealed class DeleteRoomCommandHandlerTests
{
    [Fact]
    /// <summary>
    /// Varmistaa, että handler palauttaa NotFound-virheen,
    /// kun huonetta ei löydy.
    /// </summary>
    public async Task Handle_ShouldReturnNotFound_WhenRoomDoesNotExist()
    {
        // Arrange
        var roomRepo = new Mock<IRoomRepository>();
        var reservationRepo = new Mock<IReservationRepository>();

        roomRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Room?)null);

        var handler = new DeleteRoomCommandHandler(
            roomRepo.Object,
            reservationRepo.Object);

        // Act
        var result = await handler.Handle(new DeleteRoomCommand(1), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);

        reservationRepo.Verify(x => x.GetByRoomIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    /// <summary>
    /// Varmistaa, että handler palauttaa Conflict-virheen,
    /// kun huone on jo inaktiivinen.
    /// </summary>
    public async Task Handle_ShouldReturnConflict_WhenRoomIsAlreadyInactive()
    {
        // Arrange
        var room = new Room("101", RoomType.Standard, 2, 100m, "Test");
        room.Deactivate();

        var roomRepo = new Mock<IRoomRepository>();
        var reservationRepo = new Mock<IReservationRepository>();

        roomRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(room);

        var handler = new DeleteRoomCommandHandler(
            roomRepo.Object,
            reservationRepo.Object);

        // Act
        var result = await handler.Handle(new DeleteRoomCommand(1), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);

        reservationRepo.Verify(x => x.GetByRoomIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    /// <summary>
    /// Varmistaa, että handler estää deaktivoinnin,
    /// jos huoneella on tuleva aktiivinen varaus.
    /// </summary>
    public async Task Handle_ShouldReturnConflict_WhenFutureActiveReservationExists()
    {
        // Arrange
        var room = new Room("101", RoomType.Standard, 2, 100m, "Test");

        var futureReservation = new Reservation(
            1, 1,
            DateTime.UtcNow.AddDays(5),
            DateTime.UtcNow.AddDays(7),
            2,
            200m,
            "Future");

        var roomRepo = new Mock<IRoomRepository>();
        var reservationRepo = new Mock<IReservationRepository>();

        roomRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(room);

        reservationRepo.Setup(x => x.GetByRoomIdAsync(1))
            .ReturnsAsync(new List<Reservation> { futureReservation });

        var handler = new DeleteRoomCommandHandler(
            roomRepo.Object,
            reservationRepo.Object);

        // Act
        var result = await handler.Handle(new DeleteRoomCommand(1), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);

        room.IsActive.Should().BeTrue();
    }

    [Fact]
    /// <summary>
    /// Varmistaa, että huone voidaan deaktivoida,
    /// jos sillä on vain käynnissä oleva varaus.
    /// </summary>
    public async Task Handle_ShouldDeactivateRoom_WhenOnlyOngoingReservationExists()
    {
        // Arrange
        var room = new Room("101", RoomType.Standard, 2, 100m, "Test");

        var ongoingReservation = new Reservation(
            1, 1,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1),
            2,
            200m,
            "Ongoing");

        var roomRepo = new Mock<IRoomRepository>();
        var reservationRepo = new Mock<IReservationRepository>();

        roomRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(room);

        reservationRepo.Setup(x => x.GetByRoomIdAsync(1))
            .ReturnsAsync(new List<Reservation> { ongoingReservation });

        roomRepo.Setup(x => x.UpdateAsync(It.IsAny<Room>()))
            .ReturnsAsync((Room r) => r);

        var handler = new DeleteRoomCommandHandler(
            roomRepo.Object,
            reservationRepo.Object);

        // Act
        var result = await handler.Handle(new DeleteRoomCommand(1), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        room.IsActive.Should().BeFalse();
    }

    [Fact]
    /// <summary>
    /// Varmistaa, että huone voidaan deaktivoida,
    /// kun sillä ei ole lainkaan varauksia.
    /// </summary>
    public async Task Handle_ShouldDeactivateRoom_WhenRoomHasNoReservations()
    {
        // Arrange
        var room = new Room("101", RoomType.Standard, 2, 100m, "Test");

        var roomRepo = new Mock<IRoomRepository>();
        var reservationRepo = new Mock<IReservationRepository>();

        roomRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(room);

        reservationRepo.Setup(x => x.GetByRoomIdAsync(1))
            .ReturnsAsync(new List<Reservation>());

        roomRepo.Setup(x => x.UpdateAsync(It.IsAny<Room>()))
            .ReturnsAsync((Room r) => r);

        var handler = new DeleteRoomCommandHandler(
            roomRepo.Object,
            reservationRepo.Object);

        // Act
        var result = await handler.Handle(new DeleteRoomCommand(1), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        room.IsActive.Should().BeFalse();
    }
}