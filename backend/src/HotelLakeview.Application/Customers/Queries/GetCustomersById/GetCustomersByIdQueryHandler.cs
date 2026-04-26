using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Customers.DTOs;
using HotelLakeview.Application.Interfaces;
using MediatR;

namespace HotelLakeview.Application.Customers.Queries.GetCustomerById;

/// <summary>
/// Käsittelijä asiakkaan hakemiselle tunnisteen perusteella.
/// </summary>
public sealed class GetCustomerByIdQueryHandler
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Luo uuden handlerin asiakkaan hakua varten.
    /// </summary>
    /// <param name="customerRepository">Asiakasrepository.</param>
    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// Hakee asiakkaan tunnisteen perusteella.
    /// </summary>
    /// <param name="request">Hakukysely.</param>
    /// <param name="cancellationToken">Peruutustunniste.</param>
    /// <returns>
    /// Löydetty asiakas DTO-muodossa tai NotFound-virhe.
    /// </returns>
    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByIdQuery request,
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

        var customerDto = new CustomerDto
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.Email ?? string.Empty,
            PhoneNumber = customer.PhoneNumber ?? string.Empty,
            Notes = customer.Notes,
            IsActive = customer.IsActive
        };

        return Result<CustomerDto>.Success(customerDto);
    }
}