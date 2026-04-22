using HotelLakeview.Application.Analytics.Dtos;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Enums;
using MediatR;

namespace HotelLakeview.Application.Analytics.Queries.GetReservationSummaryReport;

public sealed class GetReservationSummaryReportQueryHandler
    : IRequestHandler<GetReservationSummaryReportQuery, Result<ReservationSummaryReportDto>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationSummaryReportQueryHandler(
        IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<ReservationSummaryReportDto>> Handle(
        GetReservationSummaryReportQuery request,
        CancellationToken cancellationToken)
    {
        var startDate = request.StartDate.Date;
        var endDate = request.EndDate.Date;

        // Haetaan kaikki varaukset jotka leikkaavat aikaväliä
        var reservations = await _reservationRepository
            .GetAllByDateRangeAsync(startDate, endDate);

        // ⚠️ TÄRKEÄ HUOMIO:
        // tämä metodi palauttaa vain aktiiviset varaukset
        // joten tarvitaan myöhemmin uusi metodi:
        // GetAllByDateRangeAsync (sisältää myös perutut)

        var activeReservations = reservations
            .Where(r => r.Status != ReservationStatus.Cancelled)
            .ToList();

        var cancelledReservations = reservations
            .Where(r => r.Status == ReservationStatus.Cancelled)
            .ToList();

        var activeCount = activeReservations.Count;
        var cancelledCount = cancelledReservations.Count;
        var totalCount = activeCount + cancelledCount;

        var cancellationRate = totalCount == 0
            ? 0
            : Math.Round((decimal)cancelledCount / totalCount * 100m, 2);

        var cancelledRevenue = cancelledReservations.Sum(r => r.TotalPrice);

        var dto = new ReservationSummaryReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            ActiveReservationCount = activeCount,
            CancelledReservationCount = cancelledCount,
            TotalReservationCount = totalCount,
            CancellationRatePercentage = cancellationRate,
            CancelledRevenueValue = cancelledRevenue
        };

        return Result<ReservationSummaryReportDto>.Success(dto);
    }
}