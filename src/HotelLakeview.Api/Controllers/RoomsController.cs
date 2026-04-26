using HotelLakeview.Api.Extensions;
using HotelLakeview.Application.Rooms.Queries.GetAvailableRooms;
using HotelLakeview.Application.Rooms.Commands.CreateRoom;
using HotelLakeview.Application.Rooms.Queries.GetRoomsPaged;
using HotelLakeview.Application.Rooms.Queries.GetRoomById;
using HotelLakeview.Application.Rooms.Commands.UpdateRoom;
using HotelLakeview.Application.Rooms.Commands.DeleteRoom;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HotelLakeview.Application.RoomImages.Commands.UploadRoomImage;
using HotelLakeview.Application.RoomImages.Queries.GetRoomImages;
using HotelLakeview.Application.RoomImages.Commands.DeleteRoomImage;

namespace HotelLakeview.Api.Controllers;

/// <summary>
/// Huoneisiin liittyvät endpointit.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Hakee vapaat huoneet annetulle aikavälille.
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableRooms(
        [FromQuery] DateTime checkInDate,
        [FromQuery] DateTime checkOutDate,
        [FromQuery] int guestCount)
    {
        var query = new GetAvailableRoomsQuery
        {
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            GuestCount = guestCount
        };

        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoomById(int id)
    {
        var query = new GetRoomByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }
        /// <summary>
    /// Luo uuden huoneen.
    /// </summary>
    /// <param name="command">Huoneen luontikomento.</param>
    /// <returns>Luotu huone.</returns>
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
    /// Hakee kaikki huoneet sivutettuna.
    /// </summary>
    /// <param name="page">Sivunumero.</param>
    /// <param name="pageSize">Sivukoko.</param>
    /// <returns>Sivutettu lista huoneista.</returns>
    [HttpGet]
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
    /// Päivittää olemassa olevan huoneen.
    /// </summary>
    /// <param name="id">Päivitettävän huoneen tunniste.</param>
    /// <param name="command">Huoneen päivityskomento.</param>
    /// <returns>Päivitetty huone.</returns>
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
    /// <param name="id">Huoneen tunniste.</param>
    /// <returns>Tyhjä vastaus, jos poisto onnistui.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var command = new DeleteRoomCommand(id);
        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }/// <summary>
    /// Hakee huoneeseen liitetyt kuvat.
    /// </summary>
    /// <param name="roomId">Huoneen tunniste.</param>
    [HttpGet("{roomId:int}/images")]
    public async Task<IActionResult> GetRoomImages(int roomId)
    {
        var query = new GetRoomImagesQuery(roomId);

        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }
    /// <summary>
    /// Lataa uuden kuvan huoneelle.
    /// </summary>
    /// <param name="roomId">Huoneen tunniste.</param>
    /// <param name="file">Ladattava kuvatiedosto.</param>
    /// <param name="sortOrder">Kuvan järjestysnumero.</param>
    /// <param name="isPrimary">Kertoo, onko kuva huoneen pääkuva.</param>
    [HttpPost("{roomId:int}/images")]
    [Consumes("multipart/form-data")]
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
    /// <param name="imageId">Poistettavan kuvan tunniste.</param>
    [HttpDelete("images/{imageId:int}")]
    public async Task<IActionResult> DeleteRoomImage(int imageId)
    {
        var command = new DeleteRoomImageCommand(imageId);

        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }
}