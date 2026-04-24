using FluentAssertions;
using HotelLakeview.Application.Auth.Interfaces;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Users.Commands.CreateUser;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Users.Commands.CreateUser;

/// <summary>
/// Testit käyttäjän luonnin käsittelijälle.
/// </summary>
public sealed class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenUsernameAlreadyExists()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var passwordHasherMock = new Mock<IPasswordHasher>();

        var existingUser = new User("Admin", "hashed-password", UserRole.Management);

        userRepositoryMock
            .Setup(x => x.GetByUsernameAsync("Admin"))
            .ReturnsAsync(existingUser);

        var handler = new CreateUserCommandHandler(
            userRepositoryMock.Object,
            passwordHasherMock.Object);

        var command = new CreateUserCommand("Admin", "Admin1", "Management");

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("User.UsernameExists");
    }
}