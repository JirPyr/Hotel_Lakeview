using FluentAssertions;
using HotelLakeview.Api.Controllers;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Reservations.Dtos;
using HotelLakeview.Application.Reservations.Queries.GetReservationById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HotelLakeview.Api.Tests.Controllers;

/// <summary>
/// Yksikkötestit ReservationsControllerille.
/// </summary>
public sealed class ReservationsControllerTests
{
    /// <summary>
    /// Varmistaa, että controller palauttaa 404 NotFound,
    /// kun varausta ei löydy.
    /// </summary>
    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenReservationDoesNotExist()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();

        mediatorMock
            .Setup(x => x.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ReservationDto>.Failure(
                Error.NotFound("Reservation.NotFound", "Varausta ei löytynyt.")));

        var controller = new ReservationsController(mediatorMock.Object);

        // Act
        var result = await controller.GetReservationById(1);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    /// <summary>
    /// Varmistaa, että controller palauttaa 200 OK,
    /// kun varaus löytyy onnistuneesti.
    /// </summary>
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenReservationExists()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();

        var dto = new ReservationDto
        {
            Id = 1,
            CustomerId = 1,
            RoomId = 1,
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(2),
            GuestCount = 2,
            TotalPrice = 100m,
            Notes = "Testi"
        };

        mediatorMock
            .Setup(x => x.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ReservationDto>.Success(dto));

        var controller = new ReservationsController(mediatorMock.Object);

        // Act
        var result = await controller.GetReservationById(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}