using FluentAssertions;
using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Users.Dtos;
using HotelLakeview.Application.Users.Queries.GetUsers;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Moq;
using Xunit;

namespace HotelLakeview.Application.Tests.Users.Queries.GetUsers;

/// <summary>
/// Testit käyttäjien listauksen käsittelijälle.
/// </summary>
public sealed class GetUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPagedUsers_WhenRequestIsValid()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();

        var users = new List<User>
        {
            new("Admin", "hash1", UserRole.Management),
            new("Reception", "hash2", UserRole.Receptionist)
        };

        userRepositoryMock
            .Setup(x => x.GetPagedAsync(1, 10))
            .ReturnsAsync(users.AsReadOnly());

        userRepositoryMock
            .Setup(x => x.CountAsync())
            .ReturnsAsync(2);

        var handler = new GetUsersQueryHandler(userRepositoryMock.Object);
        var query = new GetUsersQuery(1, 10);

        // Act
        Result<PagedResult<UserDto>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalCount.Should().Be(2);

        result.Value.Items.First().Username.Should().Be("Admin");
    }
}