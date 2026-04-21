using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Customers.DTOs;
using MediatR;

namespace HotelLakeview.Application.Customers.Queries.GetCustomerById;

/// <summary>
/// Kysely yksittäisen asiakkaan hakemiseksi tunnisteen perusteella.
/// </summary>
/// <param name="Id">Haettavan asiakkaan tunniste.</param>
public sealed record GetCustomerByIdQuery(int Id) : IRequest<Result<CustomerDto>>;