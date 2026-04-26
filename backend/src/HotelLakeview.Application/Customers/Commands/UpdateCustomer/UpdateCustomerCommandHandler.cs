using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Customers.DTOs;
using HotelLakeview.Application.Interfaces;
using MediatR;

namespace HotelLakeview.Application.Customers.Commands.UpdateCustomer;

/// <summary>
/// Käsittelijä asiakkaan päivittämiselle.
/// </summary>
public sealed class UpdateCustomerCommandHandler
    : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Luo uuden handlerin asiakkaan päivittämistä varten.
    /// </summary>
    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <inheritdoc />
    public async Task<Result<CustomerDto>> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);

        if (customer is null)
        {
            return Result<CustomerDto>.Failure(
                Error.NotFound(
                    "Customer.NotFound",
                    $"Customer with id {request.Id} was not found."));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingCustomerWithEmail = await _customerRepository.GetByEmailAsync(request.Email);

            if (existingCustomerWithEmail is not null &&
                existingCustomerWithEmail.Id != request.Id &&
                existingCustomerWithEmail.IsActive)
            {
                return Result<CustomerDto>.Failure(
                    Error.Conflict(
                        "Customer.EmailAlreadyInUse",
                        $"Email '{request.Email}' is already in use."));
            }
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var existingCustomerWithPhoneNumber = await _customerRepository.GetByPhoneNumberAsync(request.PhoneNumber);

            if (existingCustomerWithPhoneNumber is not null &&
                existingCustomerWithPhoneNumber.Id != request.Id &&
                existingCustomerWithPhoneNumber.IsActive)
            {
                return Result<CustomerDto>.Failure(
                    Error.Conflict(
                        "Customer.PhoneNumberAlreadyInUse",
                        $"Phone number '{request.PhoneNumber}' is already in use."));
            }
        }

        customer.UpdateDetails(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Notes);

        var updatedCustomer = await _customerRepository.UpdateAsync(customer);

        var customerDto = new CustomerDto
        {
            Id = updatedCustomer.Id,
            FullName = updatedCustomer.FullName,
            Email = updatedCustomer.Email ?? string.Empty,
            PhoneNumber = updatedCustomer.PhoneNumber ?? string.Empty,
            Notes = updatedCustomer.Notes,
            IsActive = updatedCustomer.IsActive
        };

        return Result<CustomerDto>.Success(customerDto);
    }
}