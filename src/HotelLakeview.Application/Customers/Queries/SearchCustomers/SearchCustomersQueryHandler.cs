using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Customers.DTOs;
using HotelLakeview.Application.Interfaces;
using MediatR;

namespace HotelLakeview.Application.Customers.Queries.SearchCustomers;

/// <summary>
/// Käsittelijä asiakkaiden hakemiselle hakusanalla.
/// </summary>
public sealed class SearchCustomersQueryHandler
    : IRequestHandler<SearchCustomersQuery, Result<PagedResult<CustomerDto>>>
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Luo käsittelijän asiakkaiden hakemista varten.
    /// </summary>
    /// <param name="customerRepository">Asiakasrepository.</param>
    public SearchCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<CustomerDto>>> Handle(
        SearchCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var pagination = new PaginationRequest
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        var customers = await _customerRepository.SearchAsync(
            request.SearchTerm,
            pagination);

        var totalCount = await _customerRepository.CountSearchResultsAsync(
            request.SearchTerm);

        var customerDtos = customers
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email ?? string.Empty,
                PhoneNumber = c.PhoneNumber ?? string.Empty,
                Notes = c.Notes,
                IsActive = c.IsActive
            })
            .ToList();

        var pagedResult = new PagedResult<CustomerDto>(
            customerDtos,
            request.Page,
            request.PageSize,
            totalCount);

        return Result<PagedResult<CustomerDto>>.Success(pagedResult);
    }
}