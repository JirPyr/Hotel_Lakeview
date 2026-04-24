using HotelLakeview.Api.Extensions;
using HotelLakeview.Application.Users.Commands.CreateUser;
using HotelLakeview.Application.Users.Commands.DeactivateUser;
using HotelLakeview.Application.Users.Dtos;
using HotelLakeview.Application.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

/// <summary>
/// Controller käyttäjien hallintaan.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Management")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Luo uuden käyttäjän.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var command = new CreateUserCommand(
            dto.Username,
            dto.Password,
            dto.Role);

        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Hakee käyttäjät sivutettuna.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersQuery(page, pageSize);
        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Deaktivoi käyttäjän.
    /// </summary>
    [HttpPatch("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var command = new DeactivateUserCommand(id);
        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }
}