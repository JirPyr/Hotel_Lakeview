using HotelLakeview.Application.Analytics.Dtos;
using HotelLakeview.Application.Common.Results;
using MediatR;

namespace HotelLakeview.Application.Analytics.Queries.GetOccupancyReport;
public sealed record GetOccupancyReportQuery(
    DateTime StartDate,
    DateTime EndDate
) : IRequest<Result<OccupancyReportDto>>;