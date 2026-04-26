using HotelLakeview.Application.Analytics.Queries.GetReservationSummaryReport;
using HotelLakeview.Application.Analytics.Queries.GetRevenueReport;
using HotelLakeview.Application.Analytics.Queries.GetPopularRoomTypesReport;
using HotelLakeview.Application.Analytics.Queries.GetOccupancyReport;   
using HotelLakeview.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.API.Controllers;

[ApiController]
[Authorize(Roles = "Management")]
[Route("api/[controller]")]
public sealed class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("reservation-summary")]
    public async Task<IActionResult> GetReservationSummary(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetReservationSummaryReportQuery(startDate, endDate),
            cancellationToken);

        return result.ToActionResult(this);
    }
    [HttpGet("occupancy")]
    public async Task<IActionResult> GetOccupancy(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await _mediator.Send(
            new GetOccupancyReportQuery(startDate, endDate));

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
       [HttpGet("revenue")]
    
    public async Task<IActionResult> GetRevenue(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetRevenueReportQuery(startDate, endDate),
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
    /// <summary>
    /// Hakee suosituimmat huonetyypit aikaväliltä.
    /// </summary>
    [HttpGet("popular-room-types")]
    public async Task<IActionResult> GetPopularRoomTypes(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPopularRoomTypesReportQuery(startDate, endDate),
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
    
}