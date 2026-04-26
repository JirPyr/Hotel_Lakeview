using FluentValidation;

namespace HotelLakeview.Application.Analytics.Queries.GetPopularRoomTypesReport;

/// <summary>
/// Validoi suosituimpien huonetyyppien raportin kyselyn.
/// </summary>
public sealed class GetPopularRoomTypesReportQueryValidator
    : AbstractValidator<GetPopularRoomTypesReportQuery>
{
    public GetPopularRoomTypesReportQueryValidator()
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