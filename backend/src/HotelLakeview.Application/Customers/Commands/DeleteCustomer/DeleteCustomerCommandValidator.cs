using FluentValidation;

namespace HotelLakeview.Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Validoi asiakkaan poistokomennon.
/// </summary>
public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Virheellinen asiakkaan tunniste.");
    }
}