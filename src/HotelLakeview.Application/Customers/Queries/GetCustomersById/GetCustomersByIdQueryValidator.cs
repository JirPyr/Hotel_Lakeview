using FluentValidation;

namespace HotelLakeview.Application.Customers.Queries.GetCustomerById;

/// <summary>
/// Validointi asiakkaan hakemiselle tunnisteella.
/// </summary>
public sealed class GetCustomerByIdQueryValidator
    : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Customer id must be greater than 0.");
    }
}