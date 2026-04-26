using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Users.Dtos;
using MediatR;

namespace HotelLakeview.Application.Users.Queries.GetUsers;

/// <summary>
/// Kysely käyttäjien hakemiseen sivutettuna.
/// </summary>
public sealed record GetUsersQuery(int Page, int PageSize)
    : IRequest<Result<PagedResult<UserDto>>>;