using FluentAssertions;
using HotelLakeview.Api.Controllers;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Users.Commands.DeactivateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HotelLakeview.API.Tests.Controllers;

/// <summary>
/// Testit UsersControllerille.
/// </summary>
public sealed class UsersControllerTests
{
    [Fact]
    public async Task DeactivateUser_ShouldReturnConflict_WhenUserIsAlreadyInactive()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();

        mediatorMock
            .Setup(x => x.Send(It.IsAny<DeactivateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(
                Error.Conflict("User.AlreadyInactive", "Käyttäjä on jo passiivinen.")));

        var controller = new UsersController(mediatorMock.Object);

        // Act
        IActionResult actionResult = await controller.DeactivateUser(1);

        // Assert
        var conflictResult = actionResult.Should().BeOfType<ConflictObjectResult>().Subject;
conflictResult.StatusCode.Should().Be(409);
    }
}