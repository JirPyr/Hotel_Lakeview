namespace HotelLakeview.Application.Analytics.Dtos;

public sealed class OccupancyReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int ActiveRoomCount { get; set; }
    public int TotalDays { get; set; }

    public int TotalAvailableRoomNights { get; set; }
    public int OccupiedRoomNights { get; set; }

    public decimal OccupancyRatePercentage { get; set; }
}