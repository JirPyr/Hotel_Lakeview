using HotelLakeview.Application.Analytics.Dtos;
using HotelLakeview.Application.Common.Results;
using MediatR;

namespace HotelLakeview.Application.Analytics.Queries.GetRevenueReport;

/// <summary>
/// Kysely liikevaihtoraportin hakemiseen aikaväliltä.
/// </summary>
public sealed record GetRevenueReportQuery(
    DateTime StartDate,
    DateTime EndDate)
    : IRequest<Result<RevenueReportDto>>;