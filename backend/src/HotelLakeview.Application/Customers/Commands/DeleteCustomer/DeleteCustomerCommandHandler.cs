using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Interfaces;
using MediatR;

namespace HotelLakeview.Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Käsittelijä asiakkaan poistamiselle.
/// </summary>
public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Luo käsittelijän asiakkaan poistamista varten.
    /// </summary>
    /// <param name="customerRepository">Asiakasrepository.</param>
    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        DeleteCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);

        if (customer is null)
        {
            return Result.Failure(
                Error.NotFound("Customer.NotFound", "Asiakasta ei löytynyt."));
        }

        await _customerRepository.DeleteAsync(request.Id);

        return Result.Success();
    }
}