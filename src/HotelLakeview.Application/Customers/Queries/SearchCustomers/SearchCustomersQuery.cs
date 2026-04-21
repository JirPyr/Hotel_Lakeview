using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Customers.DTOs;
using MediatR;

namespace HotelLakeview.Application.Customers.Queries.SearchCustomers;

/// <summary>
/// Kysely asiakkaiden hakemiseen hakusanalla.
/// </summary>
public sealed record SearchCustomersQuery(
    string SearchTerm,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedResult<CustomerDto>>>;