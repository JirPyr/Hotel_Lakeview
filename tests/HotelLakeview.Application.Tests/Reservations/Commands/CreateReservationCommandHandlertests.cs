using FluentAssertions;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Reservations.Commands.CreateReservation;
using HotelLakeview.Application.Reservations.Services;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Reservations.Commands;

/// <summary>
/// Yksikkötestit varauksen luomisen käsittelijälle.
/// </summary>
public sealed class CreateReservationCommandHandlerTests
{
    [Fact]
    /// <summary>
    /// Varmistaa, että handler palauttaa NotFound-virheen,
    /// kun asiakasta ei löydy.
    /// </summary>
    public async Task Handle_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        var reservationRepo = new Mock<IReservationRepository>();
        var customerRepo = new Mock<ICustomerRepository>();
        var roomRepo = new Mock<IRoomRepository>();
        var pricing = new Mock<IReservationPricingService>();

        customerRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Customer?)null);

        var handler = new CreateReservationCommandHandler(
            reservationRepo.Object,
            customerRepo.Object,
            roomRepo.Object,
            pricing.Object);

        var command = new CreateReservationCommand(
            1, 1,
            DateTime.Today,
            DateTime.Today.AddDays(2),
            2,
            "Test");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    /// <summary>
    /// Varmistaa, että handler palauttaa NotFound-virheen,
    /// kun huonetta ei löydy.
    /// </summary>
    public async Task Handle_ShouldReturnNotFound_WhenRoomDoesNotExist()
    {
        var reservationRepo = new Mock<IReservationRepository>();
        var customerRepo = new Mock<ICustomerRepository>();
        var roomRepo = new Mock<IRoomRepository>();
        var pricing = new Mock<IReservationPricingService>();

        var customer = new Customer("Test", "test@test.com", "123","Testi");

        customerRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(customer);

        roomRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Room?)null);

        var handler = new CreateReservationCommandHandler(
            reservationRepo.Object,
            customerRepo.Object,
            roomRepo.Object,
            pricing.Object);

        var command = new CreateReservationCommand(
            1, 1,
            DateTime.Today,
            DateTime.Today.AddDays(2),
            2,
            "Test");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    /// <summary>
    /// Varmistaa, että handler estää varauksen,
    /// jos huone ei ole aktiivinen.
    /// </summary>
    public async Task Handle_ShouldReturnConflict_WhenRoomIsInactive()
    {
        var reservationRepo = new Mock<IReservationRepository>();
        var customerRepo = new Mock<ICustomerRepository>();
        var roomRepo = new Mock<IRoomRepository>();
        var pricing = new Mock<IReservationPricingService>();

        var customer = new Customer("Test", "test@test.com", "123","Testi");

        var room = new Room("101", RoomType.Standard, 2, 100m, "Test");
        room.Deactivate();

        customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
        roomRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(room);

        var handler = new CreateReservationCommandHandler(
            reservationRepo.Object,
            customerRepo.Object,
            roomRepo.Object,
            pricing.Object);

        var command = new CreateReservationCommand(
            1, 1,
            DateTime.Today,
            DateTime.Today.AddDays(2),
            2,
            "Test");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    /// <summary>
    /// Varmistaa, että handler palauttaa Validation-virheen,
    /// kun henkilömäärä ylittää huoneen kapasiteetin.
    /// </summary>
    public async Task Handle_ShouldReturnValidation_WhenGuestCountExceedsCapacity()
    {
        var reservationRepo = new Mock<IReservationRepository>();
        var customerRepo = new Mock<ICustomerRepository>();
        var roomRepo = new Mock<IRoomRepository>();
        var pricing = new Mock<IReservationPricingService>();

        var customer = new Customer("Test", "test@test.com", "123","Testi");
        var room = new Room("101", RoomType.Standard, 2, 100m, "Test");

        customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
        roomRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(room);

        var handler = new CreateReservationCommandHandler(
            reservationRepo.Object,
            customerRepo.Object,
            roomRepo.Object,
            pricing.Object);

        var command = new CreateReservationCommand(
            1, 1,
            DateTime.Today,
            DateTime.Today.AddDays(2),
            5,
            "Test");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    /// <summary>
    /// Varmistaa, että handler estää varauksen,
    /// jos aikavälille löytyy päällekkäinen varaus.
    /// </summary>
    public async Task Handle_ShouldReturnConflict_WhenOverlappingReservationExists()
    {
        var reservationRepo = new Mock<IReservationRepository>();
        var customerRepo = new Mock<ICustomerRepository>();
        var roomRepo = new Mock<IRoomRepository>();
        var pricing = new Mock<IReservationPricingService>();

        var customer = new Customer("Test", "test@test.com", "123","Testi");
        var room = new Room("101", RoomType.Standard, 2, 100m, "Test");

        customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
        roomRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(room);

        reservationRepo.Setup(x => x.HasOverlappingReservationAsync(
            It.IsAny<int>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        var handler = new CreateReservationCommandHandler(
            reservationRepo.Object,
            customerRepo.Object,
            roomRepo.Object,
            pricing.Object);

        var command = new CreateReservationCommand(
            1, 1,
            DateTime.Today,
            DateTime.Today.AddDays(2),
            2,
            "Test");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    /// <summary>
    /// Varmistaa, että varaus luodaan onnistuneesti,
    /// kun kaikki tiedot ovat valideja.
    /// </summary>
    public async Task Handle_ShouldCreateReservation_WhenRequestIsValid()
    {
        var reservationRepo = new Mock<IReservationRepository>();
        var customerRepo = new Mock<ICustomerRepository>();
        var roomRepo = new Mock<IRoomRepository>();
        var pricing = new Mock<IReservationPricingService>();

        var customer = new Customer("Test", "test@test.com", "123","Testi");
        var room = new Room("101", RoomType.Standard, 4, 100m, "Test");

        customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
        roomRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(room);

        reservationRepo.Setup(x => x.HasOverlappingReservationAsync(
            It.IsAny<int>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>()))
            .ReturnsAsync(false);

        pricing.Setup(x => x.CalculateTotalPrice(
            It.IsAny<decimal>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>()))
            .Returns(300m);

        reservationRepo.Setup(x => x.AddAsync(It.IsAny<Reservation>()))
            .ReturnsAsync((Reservation r) => r);

        var handler = new CreateReservationCommandHandler(
            reservationRepo.Object,
            customerRepo.Object,
            roomRepo.Object,
            pricing.Object);

        var command = new CreateReservationCommand(
            1, 1,
            DateTime.Today,
            DateTime.Today.AddDays(2),
            3,
            "Test");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        reservationRepo.Verify(
            x => x.AddAsync(It.IsAny<Reservation>()),
            Times.Once);
    }
}