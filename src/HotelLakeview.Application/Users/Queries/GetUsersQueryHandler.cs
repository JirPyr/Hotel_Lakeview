using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Application.Users.Dtos;
using MediatR;

namespace HotelLakeview.Application.Users.Queries.GetUsers;

/// <summary>
/// Käsittelijä käyttäjien hakemiseen sivutettuna.
/// </summary>
public sealed class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<PagedResult<UserDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetPagedAsync(request.Page, request.PageSize);
        var totalCount = await _userRepository.CountAsync();

        var items = users
            .Select(user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAtUtc = user.CreatedAtUtc
            })
            .ToList()
            .AsReadOnly();

        var result = new PagedResult<UserDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount);

        return Result<PagedResult<UserDto>>.Success(result);
    }
}