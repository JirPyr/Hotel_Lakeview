using FluentValidation;

namespace HotelLakeview.Application.Users.Queries.GetUsers;

/// <summary>
/// Validoi käyttäjien listauskyselyn.
/// </summary>
public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}