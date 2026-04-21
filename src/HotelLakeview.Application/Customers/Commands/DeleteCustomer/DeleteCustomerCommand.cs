using HotelLakeview.Application.Common.Results;
using MediatR;

namespace HotelLakeview.Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Komento asiakkaan passivoimiseen.
/// </summary>
/// <param name="Id">Passivoitavan asiakkaan tunniste.</param>
public sealed record DeleteCustomerCommand(int Id) : IRequest<Result>;