using HotelLakeview.Api.Extensions;
using HotelLakeview.Application.Rooms.Commands.CreateRoom;
using HotelLakeview.Application.Rooms.Commands.DeleteRoom;
using HotelLakeview.Application.Rooms.Commands.UpdateRoom;
using HotelLakeview.Application.Rooms.Queries.GetAvailableRooms;
using HotelLakeview.Application.Rooms.Queries.GetRoomById;
using HotelLakeview.Application.Rooms.Queries.GetRoomsPaged;
using HotelLakeview.Application.RoomImages.Commands.DeleteRoomImage;
using HotelLakeview.Application.RoomImages.Commands.UploadRoomImage;
using HotelLakeview.Application.RoomImages.Queries.GetRoomImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

/// <summary>
/// Huoneisiin liittyvät endpointit.
/// </summary>
[ApiController]
[Authorize(Roles = "Management,Receptionist")]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Luo uuden RoomsController-instanssin.
    /// </summary>
    public RoomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Hakee kaikki huoneet sivutettuna.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRooms(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetRoomsPagedQuery(page, pageSize);
        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Hakee yksittäisen huoneen tunnisteen perusteella.
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoomById(int id)
    {
        var query = new GetRoomByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Hakee vapaat huoneet annetulle aikavälille ja henkilömäärälle.
    /// </summary>
    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableRooms(
        [FromQuery] DateTime checkInDate,
        [FromQuery] DateTime checkOutDate,
        [FromQuery] int guestCount)
    {
        var utcCheckInDate = DateTime.SpecifyKind(checkInDate.Date, DateTimeKind.Utc);
        var utcCheckOutDate = DateTime.SpecifyKind(checkOutDate.Date, DateTimeKind.Utc);

        var query = new GetAvailableRoomsQuery
        {
            CheckInDate = utcCheckInDate,
            CheckOutDate = utcCheckOutDate,
            GuestCount = guestCount
        };

        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Luo uuden huoneen.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomCommand command)
    {
        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Päivittää olemassa olevan huoneen.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Reitin id ei vastaa pyynnön id:tä.");
        }

        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Poistaa huoneen käytöstä.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var command = new DeleteRoomCommand(id);
        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Hakee huoneeseen liitetyt kuvat.
    /// </summary>
    [HttpGet("{roomId:int}/images")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoomImages(int roomId)
    {
        var query = new GetRoomImagesQuery(roomId);
        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Lataa uuden kuvan huoneelle.
    /// </summary>
    [HttpPost("{roomId:int}/images")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadRoomImage(
        int roomId,
        IFormFile file,
        [FromForm] int sortOrder = 0,
        [FromForm] bool isPrimary = false)
    {
        await using var stream = file.OpenReadStream();

        var command = new UploadRoomImageCommand(
            roomId,
            file.FileName,
            file.ContentType,
            stream,
            sortOrder,
            isPrimary);

        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Poistaa huonekuvan.
    /// </summary>
    [HttpDelete("images/{imageId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoomImage(int imageId)
    {
        var command = new DeleteRoomImageCommand(imageId);
        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }
}