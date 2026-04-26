using FluentAssertions;
using HotelLakeview.Application.Auth.Commands.Login;
using HotelLakeview.Application.Auth.Dtos;
using HotelLakeview.Application.Auth.Interfaces;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Auth.Commands.Login;

/// <summary>
/// Testit kirjautumisen käsittelijälle.
/// </summary>
public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var tokenServiceMock = new Mock<ITokenService>();

        var user = new User("Admin", "hashed-password", UserRole.Management);

        userRepositoryMock
            .Setup(x => x.GetByUsernameAsync("Admin"))
            .ReturnsAsync(user);

        passwordHasherMock
            .Setup(x => x.Verify("Admin1", "hashed-password"))
            .Returns(true);

        tokenServiceMock
            .Setup(x => x.CreateToken(user))
            .Returns("jwt-token");

        var handler = new LoginCommandHandler(
            userRepositoryMock.Object,
            passwordHasherMock.Object,
            tokenServiceMock.Object);

        var command = new LoginCommand("Admin", "Admin1");

        // Act
        Result<LoginResponseDto> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("jwt-token");
        result.Value.Username.Should().Be("Admin");
        result.Value.Role.Should().Be("Management");
    }
}