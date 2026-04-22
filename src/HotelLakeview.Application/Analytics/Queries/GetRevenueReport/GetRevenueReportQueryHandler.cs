using HotelLakeview.Application.Analytics.Dtos;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Enums;
using MediatR;

namespace HotelLakeview.Application.Analytics.Queries.GetRevenueReport;

/// <summary>
/// Käsittelijä liikevaihtoraportin hakemiseen.
/// Jakaa varauksen tuoton yökohtaisesti oikeille kuukausille.
/// </summary>
public sealed class GetRevenueReportQueryHandler
    : IRequestHandler<GetRevenueReportQuery, Result<RevenueReportDto>>
{
    private readonly IReservationRepository _reservationRepository;

    /// <summary>
    /// Luo käsittelijän liikevaihtoraporttia varten.
    /// </summary>
    public GetRevenueReportQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    /// <inheritdoc />
    public async Task<Result<RevenueReportDto>> Handle(
        GetRevenueReportQuery request,
        CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.GetActiveByDateRangeAsync(
            request.StartDate,
            request.EndDate);

        var confirmedReservations = reservations
            .Where(r => r.Status == ReservationStatus.Confirmed)
            .ToList();

        var monthlyRevenueMap = new Dictionary<(int Year, int Month), decimal>();

        foreach (var reservation in confirmedReservations)
        {
            var reservationStart = reservation.CheckInDate.Date;
            var reservationEnd = reservation.CheckOutDate.Date;

            var totalReservationNights = (reservationEnd - reservationStart).Days;

            if (totalReservationNights <= 0)
            {
                continue;
            }

            var overlapStart = reservationStart > request.StartDate.Date
                ? reservationStart
                : request.StartDate.Date;

            var overlapEnd = reservationEnd < request.EndDate.Date
                ? reservationEnd
                : request.EndDate.Date;

            if (overlapEnd <= overlapStart)
            {
                continue;
            }

            var revenuePerNight = reservation.TotalPrice / totalReservationNights;

            for (var date = overlapStart; date < overlapEnd; date = date.AddDays(1))
            {
                var key = (date.Year, date.Month);

                if (!monthlyRevenueMap.ContainsKey(key))
                {
                    monthlyRevenueMap[key] = 0m;
                }

                monthlyRevenueMap[key] += revenuePerNight;
            }
        }

        var months = monthlyRevenueMap
            .Select(x => new MonthlyRevenueDto
            {
                Year = x.Key.Year,
                Month = x.Key.Month,
                Revenue = Math.Round(x.Value, 2)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();

        var dto = new RevenueReportDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Months = months,
            TotalRevenue = Math.Round(months.Sum(x => x.Revenue), 2)
        };

        return Result<RevenueReportDto>.Success(dto);
    }
}