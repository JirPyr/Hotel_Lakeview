using HotelLakeview.Application.Analytics.Dtos;
using HotelLakeview.Application.Common.Results;
using MediatR;

namespace HotelLakeview.Application.Analytics.Queries.GetPopularRoomTypesReport;

/// <summary>
/// Kysely suosituimpien huonetyyppien raportin hakemiseen aikaväliltä.
/// </summary>
public sealed record GetPopularRoomTypesReportQuery(
    DateTime StartDate,
    DateTime EndDate)
    : IRequest<Result<PopularRoomTypesReportDto>>;