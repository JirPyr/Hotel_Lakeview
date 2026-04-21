using HotelLakeview.Application.Common.Models;
using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Customers.DTOs;
using MediatR;

namespace HotelLakeview.Application.Customers.Queries.GetCustomers;

/// <summary>
/// Kysely asiakkaiden listaamiseksi sivutettuna.
/// </summary>
/// <param name="Page">Sivunumero.</param>
/// <param name="PageSize">Sivun koko.</param>
public sealed record GetCustomersQuery(
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedResult<CustomerDto>>>;