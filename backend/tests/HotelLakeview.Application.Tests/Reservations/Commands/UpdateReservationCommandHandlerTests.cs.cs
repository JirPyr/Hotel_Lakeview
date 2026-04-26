using FluentAssertions;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Reservations.Commands.UpdateReservation;
using HotelLakeview.Application.Reservations.Dtos;
using HotelLakeview.Application.Reservations.Services;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Reservations.Commands;

/// <summary>
/// Yksikkötestit varauksen päivittämisen käsittelijälle.
/// </summary>
public sealed class UpdateReservationCommandHandlerTests
{
    /// <summary>
/// Varmistaa, että handler palauttaa NotFound-virheen,
/// kun päivitettävää varausta ei löydy annetulla tunnisteella.
///
/// Testaa, että:
/// - tulos on epäonnistunut
/// - virhetyyppi on NotFound
/// - oikea virhekoodi palautetaan
/// - huonetta ei lähdetä hakemaan turhaan
/// - varausta ei päivitetä repositoryyn
/// </summary>
[Fact]
public async Task Handle_ShouldReturnNotFound_WhenReservationDoesNotExist()
{
    // Arrange
    var reservationRepositoryMock = new Mock<IReservationRepository>();
    var roomRepositoryMock = new Mock<IRoomRepository>();
    var pricingServiceMock = new Mock<IReservationPricingService>();

    reservationRepositoryMock
        .Setup(x => x.GetByIdAsync(999))
        .ReturnsAsync((Reservation?)null);

    var handler = new UpdateReservationCommandHandler(
        reservationRepositoryMock.Object,
        roomRepositoryMock.Object,
        pricingServiceMock.Object);

    var command = new UpdateReservationCommand(
        Id: 999,
        RoomId: 1,
        CheckInDate: new DateTime(2026, 6, 10),
        CheckOutDate: new DateTime(2026, 6, 12),
        GuestCount: 2,
        Notes: "Päivitetty testivaraus");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.NotFound);
    result.Error.Code.Should().Be("Reservation.NotFound");

    roomRepositoryMock.Verify(
        x => x.GetByIdAsync(It.IsAny<int>()),
        Times.Never);

    reservationRepositoryMock.Verify(
        x => x.UpdateAsync(It.IsAny<Reservation>()),
        Times.Never);
}
/// <summary>
/// Varmistaa, että handler palauttaa Conflict-virheen,
/// kun päivitettävä varaus on jo peruttu.
///
/// Testaa, että:
/// - tulos on epäonnistunut
/// - virhetyyppi on Conflict
/// - oikea virhekoodi palautetaan
/// - huonetta ei lähdetä hakemaan turhaan
/// - varausta ei päivitetä repositoryyn
/// </summary>
[Fact]
public async Task Handle_ShouldReturnConflict_WhenReservationIsCancelled()
{
    // Arrange
    var reservationRepositoryMock = new Mock<IReservationRepository>();
    var roomRepositoryMock = new Mock<IRoomRepository>();
    var pricingServiceMock = new Mock<IReservationPricingService>();

    var reservation = new Reservation(
        customerId: 1,
        roomId: 1,
        checkInDate: new DateTime(2026, 6, 10),
        checkOutDate: new DateTime(2026, 6, 12),
        guestCount: 2,
        totalPrice: 200m,
        notes: "Peruttava testivaraus");

    reservation.Cancel();

    reservationRepositoryMock
        .Setup(x => x.GetByIdAsync(1))
        .ReturnsAsync(reservation);

    var handler = new UpdateReservationCommandHandler(
        reservationRepositoryMock.Object,
        roomRepositoryMock.Object,
        pricingServiceMock.Object);

    var command = new UpdateReservationCommand(
        Id: 1,
        RoomId: 2,
        CheckInDate: new DateTime(2026, 6, 15),
        CheckOutDate: new DateTime(2026, 6, 17),
        GuestCount: 2,
        Notes: "Yritetään muokata peruttua varausta");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.Conflict);
    result.Error.Code.Should().Be("Reservation.CancelledCannotBeModified");

    roomRepositoryMock.Verify(
        x => x.GetByIdAsync(It.IsAny<int>()),
        Times.Never);

    reservationRepositoryMock.Verify(
        x => x.HasOverlappingReservationAsync(
            It.IsAny<int>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            It.IsAny<int?>()),
        Times.Never);

    reservationRepositoryMock.Verify(
        x => x.UpdateAsync(It.IsAny<Reservation>()),
        Times.Never);
}/// <summary>
/// Varmistaa, että handler palauttaa NotFound-virheen,
/// kun päivityspyynnössä annettua huonetta ei löydy.
///
/// Testaa, että:
/// - varaus löytyy normaalisti
/// - varaus ei ole peruttu
/// - huonehaku palauttaa null-arvon
/// - tulos on epäonnistunut
/// - virhetyyppi on NotFound
/// - oikea virhekoodi palautetaan
/// - overlap-tarkistusta ei tehdä
/// - varausta ei päivitetä repositoryyn
/// </summary>
[Fact]
public async Task Handle_ShouldReturnNotFound_WhenRoomDoesNotExist()
{
    // Arrange
    var reservationRepositoryMock = new Mock<IReservationRepository>();
    var roomRepositoryMock = new Mock<IRoomRepository>();
    var pricingServiceMock = new Mock<IReservationPricingService>();

    var reservation = new Reservation(
        customerId: 1,
        roomId: 1,
        checkInDate: new DateTime(2026, 6, 10),
        checkOutDate: new DateTime(2026, 6, 12),
        guestCount: 2,
        totalPrice: 200m,
        notes: "Aktiivinen testivaraus");

    reservationRepositoryMock
        .Setup(x => x.GetByIdAsync(1))
        .ReturnsAsync(reservation);

    roomRepositoryMock
        .Setup(x => x.GetByIdAsync(999))
        .ReturnsAsync((Room?)null);

    var handler = new UpdateReservationCommandHandler(
        reservationRepositoryMock.Object,
        roomRepositoryMock.Object,
        pricingServiceMock.Object);

    var command = new UpdateReservationCommand(
        Id: 1,
        RoomId: 999,
        CheckInDate: new DateTime(2026, 6, 15),
        CheckOutDate: new DateTime(2026, 6, 17),
        GuestCount: 2,
        Notes: "Yritetään vaihtaa olemattomaan huoneeseen");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.NotFound);
    result.Error.Code.Should().Be("Room.NotFound");

    reservationRepositoryMock.Verify(
        x => x.HasOverlappingReservationAsync(
            It.IsAny<int>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            It.IsAny<int?>()),
        Times.Never);

    reservationRepositoryMock.Verify(
        x => x.UpdateAsync(It.IsAny<Reservation>()),
        Times.Never);
}/// <summary>
/// Varmistaa, että handler palauttaa Conflict-virheen,
/// kun päivityspyynnössä annettu huone on inaktiivinen.
///
/// Testaa, että:
/// - varaus löytyy normaalisti
/// - huone löytyy, mutta se ei ole aktiivinen
/// - tulos on epäonnistunut
/// - virhetyyppi on Conflict
/// - oikea virhekoodi palautetaan
/// - overlap-tarkistusta ei tehdä
/// - varausta ei päivitetä repositoryyn
/// </summary>
[Fact]
public async Task Handle_ShouldReturnConflict_WhenRoomIsInactive()
{
    // Arrange
    var reservationRepositoryMock = new Mock<IReservationRepository>();
    var roomRepositoryMock = new Mock<IRoomRepository>();
    var pricingServiceMock = new Mock<IReservationPricingService>();

    var reservation = new Reservation(
        customerId: 1,
        roomId: 1,
        checkInDate: new DateTime(2026, 6, 10),
        checkOutDate: new DateTime(2026, 6, 12),
        guestCount: 2,
        totalPrice: 200m,
        notes: "Aktiivinen testivaraus");

    var room = new Room(
        roomNumber: "101",
        roomType: RoomType.Standard,
        maxGuests: 2,
        basePricePerNight: 100m,
        description: "Testihuone");

    room.Deactivate(); // 🔥 TÄMÄ on testin ydin

    reservationRepositoryMock
        .Setup(x => x.GetByIdAsync(1))
        .ReturnsAsync(reservation);

    roomRepositoryMock
        .Setup(x => x.GetByIdAsync(2))
        .ReturnsAsync(room);

    var handler = new UpdateReservationCommandHandler(
        reservationRepositoryMock.Object,
        roomRepositoryMock.Object,
        pricingServiceMock.Object);

    var command = new UpdateReservationCommand(
        Id: 1,
        RoomId: 2,
        CheckInDate: new DateTime(2026, 6, 15),
        CheckOutDate: new DateTime(2026, 6, 17),
        GuestCount: 2,
        Notes: "Yritetään käyttää inaktiivista huonetta");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.Conflict);
    result.Error.Code.Should().Be("Room.Inactive");

    reservationRepositoryMock.Verify(
        x => x.HasOverlappingReservationAsync(
            It.IsAny<int>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            It.IsAny<int?>()),
        Times.Never);

    reservationRepositoryMock.Verify(
        x => x.UpdateAsync(It.IsAny<Reservation>()),
        Times.Never);
}
/// <summary>
/// Varmistaa, että handler palauttaa Validation-virheen,
/// kun annettu henkilömäärä ylittää huoneen kapasiteetin.
///
/// Testaa, että:
/// - varaus löytyy normaalisti
/// - huone löytyy ja on aktiivinen
/// - henkilömäärä ylittää huoneen kapasiteetin
/// - tulos on epäonnistunut
/// - virhetyyppi on Validation
/// - oikea virhekoodi palautetaan
/// - overlap-tarkistusta ei tehdä
/// - varausta ei päivitetä repositoryyn
/// </summary>
[Fact]
public async Task Handle_ShouldReturnValidation_WhenGuestCountExceedsCapacity()
{
    // Arrange
    var reservationRepositoryMock = new Mock<IReservationRepository>();
    var roomRepositoryMock = new Mock<IRoomRepository>();
    var pricingServiceMock = new Mock<IReservationPricingService>();

    var reservation = new Reservation(
        customerId: 1,
        roomId: 1,
        checkInDate: new DateTime(2026, 6, 10),
        checkOutDate: new DateTime(2026, 6, 12),
        guestCount: 2,
        totalPrice: 200m,
        notes: "Testivaraus");

    var room = new Room(
        roomNumber: "101",
        roomType: RoomType.Standard,
        maxGuests: 2, // 🔥 kapasiteetti
        basePricePerNight: 100m,
        description: "Testihuone");

    reservationRepositoryMock
        .Setup(x => x.GetByIdAsync(1))
        .ReturnsAsync(reservation);

    roomRepositoryMock
        .Setup(x => x.GetByIdAsync(2))
        .ReturnsAsync(room);

    var handler = new UpdateReservationCommandHandler(
        reservationRepositoryMock.Object,
        roomRepositoryMock.Object,
        pricingServiceMock.Object);

    var command = new UpdateReservationCommand(
        Id: 1,
        RoomId: 2,
        CheckInDate: new DateTime(2026, 6, 15),
        CheckOutDate: new DateTime(2026, 6, 17),
        GuestCount: 5, // 🔥 YLITTÄÄ kapasiteetin
        Notes: "Liikaa henkilöitä");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.Validation);
    result.Error.Code.Should().Be("Reservation.GuestCountExceeded");

    reservationRepositoryMock.Verify(
        x => x.HasOverlappingReservationAsync(
            It.IsAny<int>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            It.IsAny<int?>()),
        Times.Never);

    reservationRepositoryMock.Verify(
        x => x.UpdateAsync(It.IsAny<Reservation>()),
        Times.Never);
}/// <summary>
/// Varmistaa, että handler palauttaa Conflict-virheen,
/// kun uudelle aikavälille löytyy päällekkäinen aktiivinen varaus samasta huoneesta.
///
/// Testaa, että:
/// - varaus löytyy normaalisti
/// - huone löytyy ja on aktiivinen
/// - henkilömäärä mahtuu huoneeseen
/// - overlap-tarkistus palauttaa true
/// - tulos on epäonnistunut
/// - virhetyyppi on Conflict
/// - oikea virhekoodi palautetaan
/// - varausta ei päivitetä repositoryyn
/// </summary>
[Fact]
public async Task Handle_ShouldReturnConflict_WhenOverlappingReservationExists()
{
    // Arrange
    var reservationRepositoryMock = new Mock<IReservationRepository>();
    var roomRepositoryMock = new Mock<IRoomRepository>();
    var pricingServiceMock = new Mock<IReservationPricingService>();

    var reservation = new Reservation(
        customerId: 1,
        roomId: 1,
        checkInDate: new DateTime(2026, 6, 10),
        checkOutDate: new DateTime(2026, 6, 12),
        guestCount: 2,
        totalPrice: 200m,
        notes: "Alkuperäinen testivaraus");

    var room = new Room(
        roomNumber: "101",
        roomType: RoomType.Standard,
        maxGuests: 2,
        basePricePerNight: 100m,
        description: "Testihuone");

    reservationRepositoryMock
        .Setup(x => x.GetByIdAsync(1))
        .ReturnsAsync(reservation);

    roomRepositoryMock
        .Setup(x => x.GetByIdAsync(2))
        .ReturnsAsync(room);

    reservationRepositoryMock
        .Setup(x => x.HasOverlappingReservationAsync(
            2,
            new DateTime(2026, 6, 15),
            new DateTime(2026, 6, 17),
            1))
        .ReturnsAsync(true);

    var handler = new UpdateReservationCommandHandler(
        reservationRepositoryMock.Object,
        roomRepositoryMock.Object,
        pricingServiceMock.Object);

    var command = new UpdateReservationCommand(
        Id: 1,
        RoomId: 2,
        CheckInDate: new DateTime(2026, 6, 15),
        CheckOutDate: new DateTime(2026, 6, 17),
        GuestCount: 2,
        Notes: "Yritetään siirtää päällekkäiseen aikaan");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.Conflict);
    result.Error.Code.Should().Be("Reservation.Overlap");

    pricingServiceMock.Verify(
        x => x.CalculateTotalPrice(
            It.IsAny<decimal>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>()),
        Times.Never);

    reservationRepositoryMock.Verify(
        x => x.UpdateAsync(It.IsAny<Reservation>()),
        Times.Never);
}/// <summary>
/// Varmistaa, että handler päivittää varauksen onnistuneesti,
/// kun kaikki annetut tiedot ovat valideja.
///
/// Testaa, että:
/// - varaus löytyy ja on aktiivinen
/// - huone löytyy ja on aktiivinen
/// - henkilömäärä mahtuu huoneeseen
/// - overlap-tarkistus palauttaa false
/// - hinnanlaskenta suoritetaan
/// - varauksen tiedot päivittyvät
/// - repositoryn UpdateAsync-metodia kutsutaan kerran
/// - palautetaan onnistunut tulos oikeilla arvoilla
/// </summary>
[Fact]
public async Task Handle_ShouldUpdateReservation_WhenRequestIsValid()
{
    // Arrange
    var reservationRepositoryMock = new Mock<IReservationRepository>();
    var roomRepositoryMock = new Mock<IRoomRepository>();
    var pricingServiceMock = new Mock<IReservationPricingService>();

    var reservation = new Reservation(
        customerId: 1,
        roomId: 1,
        checkInDate: new DateTime(2026, 6, 10),
        checkOutDate: new DateTime(2026, 6, 12),
        guestCount: 2,
        totalPrice: 200m,
        notes: "Alkuperäinen");

    var room = new Room(
        roomNumber: "101",
        roomType: RoomType.Standard,
        maxGuests: 4,
        basePricePerNight: 100m,
        description: "Testihuone");

    reservationRepositoryMock
        .Setup(x => x.GetByIdAsync(1))
        .ReturnsAsync(reservation);

    roomRepositoryMock
        .Setup(x => x.GetByIdAsync(2))
        .ReturnsAsync(room);

    reservationRepositoryMock
        .Setup(x => x.HasOverlappingReservationAsync(
            It.IsAny<int>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            It.IsAny<int?>()))
        .ReturnsAsync(false);

    pricingServiceMock
        .Setup(x => x.CalculateTotalPrice(
            room.BasePricePerNight,
            new DateTime(2026, 6, 15),
            new DateTime(2026, 6, 17)))
        .Returns(300m);

    reservationRepositoryMock
        .Setup(x => x.UpdateAsync(It.IsAny<Reservation>()))
        .ReturnsAsync((Reservation r) => r);

    var handler = new UpdateReservationCommandHandler(
        reservationRepositoryMock.Object,
        roomRepositoryMock.Object,
        pricingServiceMock.Object);

    var command = new UpdateReservationCommand(
        Id: 1,
        RoomId: 2,
        CheckInDate: new DateTime(2026, 6, 15),
        CheckOutDate: new DateTime(2026, 6, 17),
        GuestCount: 3,
        Notes: "Päivitetty");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();

    var dto = result.Value;

    dto.RoomId.Should().Be(2);
    dto.GuestCount.Should().Be(3);
    dto.TotalPrice.Should().Be(300m);
    dto.Notes.Should().Be("Päivitetty");

    reservationRepositoryMock.Verify(
        x => x.UpdateAsync(It.IsAny<Reservation>()),
        Times.Once);

    pricingServiceMock.Verify(
        x => x.CalculateTotalPrice(
            room.BasePricePerNight,
            command.CheckInDate,
            command.CheckOutDate),
        Times.Once);
}
}