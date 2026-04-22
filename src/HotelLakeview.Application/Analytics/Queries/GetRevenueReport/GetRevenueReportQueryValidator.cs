using FluentValidation;

namespace HotelLakeview.Application.Analytics.Queries.GetRevenueReport;

/// <summary>
/// Validoi liikevaihtoraportin kyselyn.
/// </summary>
public sealed class GetRevenueReportQueryValidator : AbstractValidator<GetRevenueReportQuery>
{
    public GetRevenueReportQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Raportin alkupäivä on pakollinen.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("Raportin loppupäivä on pakollinen.");

        RuleFor(x => x)
            .Must(x => x.EndDate > x.StartDate)
            .WithMessage("Loppupäivän täytyy olla alkupäivän jälkeen.");
    }
}