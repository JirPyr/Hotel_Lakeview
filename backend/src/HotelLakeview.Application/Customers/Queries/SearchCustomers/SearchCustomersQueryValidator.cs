using FluentValidation;

namespace HotelLakeview.Application.Customers.Queries.SearchCustomers;

/// <summary>
/// Validoi asiakashakukyselyn.
/// </summary>
public class SearchCustomersQueryValidator : AbstractValidator<SearchCustomersQuery>
{
    public SearchCustomersQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty().WithMessage("Hakusana on pakollinen.")
            .MaximumLength(100);

        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}