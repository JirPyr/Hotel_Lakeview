using FluentValidation;

namespace HotelLakeview.Application.Customers.Queries.GetCustomers;

/// <summary>
/// Validointi asiakkaiden listaukselle.
/// </summary>
public sealed class GetCustomersQueryValidator : AbstractValidator<GetCustomersQuery>
{
    public GetCustomersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must be 100 or less.");
    }
}