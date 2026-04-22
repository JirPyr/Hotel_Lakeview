using FluentValidation;

namespace HotelLakeview.Application.Analytics.Queries.GetReservationSummaryReport;

public sealed class GetReservationSummaryReportQueryValidator
    : AbstractValidator<GetReservationSummaryReportQuery>
{
    public GetReservationSummaryReportQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date.");
    }
}