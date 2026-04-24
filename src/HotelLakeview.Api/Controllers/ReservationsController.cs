using HotelLakeview.Api.Extensions;
using HotelLakeview.Application.Reservations.Commands.CancelReservation;
using HotelLakeview.Application.Reservations.Commands.CreateReservation;
using HotelLakeview.Application.Reservations.Commands.UpdateReservation;
using HotelLakeview.Application.Reservations.Queries.GetReservationById;
using HotelLakeview.Application.Reservations.Queries.GetReservationsPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HotelLakeview.Api.Controllers;

/// <summary>
/// Controller varausten hallintaan.
/// </summary>
[ApiController]
[Authorize(Roles = "Receptionist,Management")]
[Route("api/[controller]")]
public sealed class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(this);
        }

        return CreatedAtAction(
            nameof(GetReservationById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetReservations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetReservationsPagedQuery(page, pageSize);
        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReservationById(int id)
    {
        var query = new GetReservationByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateReservation(int id, [FromBody] UpdateReservationCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Reitin id ei vastaa pyynnön id:tä.");
        }

        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var command = new CancelReservationCommand(id);
        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }
}