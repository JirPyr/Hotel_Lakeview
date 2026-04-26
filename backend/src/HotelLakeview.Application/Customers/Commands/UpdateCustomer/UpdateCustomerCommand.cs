using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Customers.DTOs;
using MediatR;

namespace HotelLakeview.Application.Customers.Commands.UpdateCustomer;

/// <summary>
/// Komento asiakkaan päivittämiseksi.
/// </summary>
/// <param name="Id">Päivitettävän asiakkaan tunniste.</param>
/// <param name="FullName">Asiakkaan koko nimi.</param>
/// <param name="Email">Asiakkaan sähköpostiosoite.</param>
/// <param name="PhoneNumber">Asiakkaan puhelinnumero.</param>
/// <param name="Notes">Mahdolliset lisätiedot.</param>
public sealed record UpdateCustomerCommand(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string? Notes
) : IRequest<Result<CustomerDto>>;