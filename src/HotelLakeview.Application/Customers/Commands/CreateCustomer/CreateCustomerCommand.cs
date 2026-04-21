using HotelLakeview.Application.Common.Results;
using HotelLakeview.Application.Customers.DTOs;
using MediatR;

namespace HotelLakeview.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Komento uuden asiakkaan luomiseksi järjestelmään.
/// 
/// Command kuvaa käyttäjän pyytämää toimenpidettä, mutta ei sisällä
/// varsinaista liiketoimintalogiikkaa. Käsittely tehdään erillisessä handlerissa.
/// </summary>
/// <param name="FullName">Asiakkaan koko nimi.</param>
/// <param name="Email">Asiakkaan sähköpostiosoite.</param>
/// <param name="PhoneNumber">Asiakkaan puhelinnumero.</param>
/// <param name="Notes">Mahdolliset lisätiedot, kuten allergiat tai erityistoiveet.</param>
public sealed record CreateCustomerCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string? Notes
) : IRequest<Result<CustomerDto>>;