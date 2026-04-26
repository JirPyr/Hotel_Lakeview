using HotelLakeview.Application.Analytics.Dtos;
using HotelLakeview.Application.Common.Results;
using MediatR;

namespace HotelLakeview.Application.Analytics.Queries.GetReservationSummaryReport;

/// <summary>
/// Query varausten yhteenvedon hakemiseen.
/// </summary>
public sealed record GetReservationSummaryReportQuery(
    DateTime StartDate,
    DateTime EndDate)
    : IRequest<Result<ReservationSummaryReportDto>>;