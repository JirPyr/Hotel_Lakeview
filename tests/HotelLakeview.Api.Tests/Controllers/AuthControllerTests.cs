using FluentAssertions;
using HotelLakeview.Api.Controllers;
using HotelLakeview.API.Controllers;
using HotelLakeview.Application.Auth.Commands.Login;
using HotelLakeview.Application.Auth.Dtos;
using HotelLakeview.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HotelLakeview.API.Tests.Controllers;

/// <summary>
/// Testit AuthControllerille.
/// </summary>
public sealed class AuthControllerTests
{
    [Fact]
    public async Task Login_ShouldReturnOk_WhenLoginSucceeds()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();

        var response = new LoginResponseDto
        {
            Token = "jwt-token",
            Username = "Admin",
            Role = "Management"
        };

        mediatorMock
            .Setup(x => x.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<LoginResponseDto>.Success(response));

        var controller = new AuthController(mediatorMock.Object);
        var command = new LoginCommand("Admin", "Admin1");

        // Act
        IActionResult actionResult = await controller.Login(command);

        // Assert
        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeAssignableTo<LoginResponseDto>().Subject;

        value.Token.Should().Be("jwt-token");
        value.Username.Should().Be("Admin");
        value.Role.Should().Be("Management");
    }
}