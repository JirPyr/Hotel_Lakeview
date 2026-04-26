using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Customers.DTOs;
using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using MediatR;

namespace HotelLakeview.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Käsittelijä uuden asiakkaan luomiselle.
/// </summary>
public sealed class CreateCustomerCommandHandler
    : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Luo uuden handlerin asiakkaan luontia varten.
    /// </summary>
    public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <inheritdoc />
    public async Task<Result<CustomerDto>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingCustomerByEmail = await _customerRepository.GetByEmailAsync(request.Email);

            if (existingCustomerByEmail is not null && existingCustomerByEmail.IsActive)
            {
                return Result<CustomerDto>.Failure(
                    Error.Conflict(
                        "Customer.EmailAlreadyExists",
                        "Sähköpostiosoite on jo käytössä."));
            }
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var existingCustomerByPhoneNumber = await _customerRepository.GetByPhoneNumberAsync(request.PhoneNumber);

            if (existingCustomerByPhoneNumber is not null && existingCustomerByPhoneNumber.IsActive)
            {
                return Result<CustomerDto>.Failure(
                    Error.Conflict(
                        "Customer.PhoneNumberAlreadyExists",
                        "Puhelinnumero on jo käytössä."));
            }
        }

        var customer = new Customer(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Notes);

        var createdCustomer = await _customerRepository.AddAsync(customer);

        var customerDto = new CustomerDto
        {
            Id = createdCustomer.Id,
            FullName = createdCustomer.FullName,
            Email = createdCustomer.Email ?? string.Empty,
            PhoneNumber = createdCustomer.PhoneNumber ?? string.Empty,
            Notes = createdCustomer.Notes,
            IsActive = createdCustomer.IsActive
        };

        return Result<CustomerDto>.Success(customerDto);
    }
}