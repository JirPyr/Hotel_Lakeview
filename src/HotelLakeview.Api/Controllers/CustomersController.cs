using HotelLakeview.Api.Extensions;
using HotelLakeview.Application.Customers.Commands.CreateCustomer;
using HotelLakeview.Application.Customers.Commands.DeleteCustomer;
using HotelLakeview.Application.Customers.Commands.UpdateCustomer;
using HotelLakeview.Application.Customers.Queries.GetCustomerById;
using HotelLakeview.Application.Customers.Queries.GetCustomers;
using HotelLakeview.Application.Customers.Queries.SearchCustomers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

/// <summary>
/// Controller asiakkaiden hallintaan.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Luo controllerin asiakkaiden hallintaa varten.
    /// </summary>
    /// <param name="mediator">MediatR-välittäjä komentojen ja kyselyiden käsittelyyn.</param>
    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Luo uuden asiakkaan.
    /// </summary>
    /// <param name="command">Asiakkaan luontikomento.</param>
    /// <returns>Luotu asiakas.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(this);
        }

        return CreatedAtAction(
            nameof(GetCustomerById),
            new { id = result.Value.Id },
            result.Value);
    }

    /// <summary>
    /// Hakee asiakkaan tunnisteen perusteella.
    /// </summary>
    /// <param name="id">Haettavan asiakkaan tunniste.</param>
    /// <returns>Asiakkaan tiedot.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Hakee asiakkaat sivutettuna.
    /// </summary>
    /// <param name="page">Sivunumero.</param>
    /// <param name="pageSize">Sivukoko.</param>
    /// <returns>Sivutettu lista asiakkaista.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCustomersQuery(page, pageSize);
        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Hakee asiakkaita hakusanalla sivutettuna.
    /// </summary>
    /// <param name="searchTerm">Hakusana nimelle, sähköpostille tai puhelinnumerolle.</param>
    /// <param name="page">Sivunumero.</param>
    /// <param name="pageSize">Sivukoko.</param>
    /// <returns>Sivutettu lista hakutuloksista.</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchCustomers(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new SearchCustomersQuery(searchTerm, page, pageSize);
        var result = await _mediator.Send(query);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Päivittää asiakkaan tiedot.
    /// </summary>
    /// <param name="id">Päivitettävän asiakkaan tunniste.</param>
    /// <param name="command">Päivityskomento.</param>
    /// <returns>Päivitetyn asiakkaan tiedot tai virhevastauksen.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCustomer(
        int id,
        [FromBody] UpdateCustomerCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new
            {
                Code = "Customer.IdMismatch",
                Message = "Reitin id ei vastaa pyynnön id:tä."
            });
        }

        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Poistaa asiakkaan.
    /// </summary>
    /// <param name="id">Poistettavan asiakkaan tunniste.</param>
    /// <returns>Tyhjä vastaus, jos poisto onnistui.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var command = new DeleteCustomerCommand(id);
        var result = await _mediator.Send(command);

        return result.ToActionResult(this);
    }
}