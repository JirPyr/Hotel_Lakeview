using FluentAssertions;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Users.Commands.DeactivateUser;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Users.Commands.DeactivateUser;

/// <summary>
/// Testit käyttäjän deaktivoinnin käsittelijälle.
/// </summary>
public sealed class DeactivateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeactivateUser_WhenUserExistsAndIsActive()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var user = new User("Admin", "hashed-password", UserRole.Management);

        userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(user);

        userRepositoryMock
            .Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(user);

        var handler = new DeactivateUserCommandHandler(userRepositoryMock.Object);
        var command = new DeactivateUserCommand(1);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();

        userRepositoryMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }
}