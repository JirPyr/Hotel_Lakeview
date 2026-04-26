using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Customers.DTOs;
using HotelLakeview.Application.Interfaces;
using MediatR;

namespace HotelLakeview.Application.Customers.Queries.GetCustomers;

/// <summary>
/// Käsittelijä asiakkaiden listaamiselle sivutettuna.
/// </summary>
public sealed class GetCustomersQueryHandler
    : IRequestHandler<GetCustomersQuery, Result<PagedResult<CustomerDto>>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<PagedResult<CustomerDto>>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetPagedAsync(request.Page, request.PageSize);
        var totalCount = await _customerRepository.CountAsync();

        var items = customers
            .Select(customer => new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email  ?? string.Empty,
                PhoneNumber = customer.PhoneNumber ?? string.Empty,
                Notes = customer.Notes,
                IsActive = customer.IsActive
            })
            .ToList();

        var pagedResult = new PagedResult<CustomerDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount);

        return Result<PagedResult<CustomerDto>>.Success(pagedResult);
    }
}