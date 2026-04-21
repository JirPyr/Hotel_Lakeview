namespace HotelLakeview.Application.Common.Results;

/// <summary>
/// Määrittelee sovelluksen virhetyypit.
/// Näitä käytetään Result Pattern -palautuksissa virheiden luokitteluun.
/// </summary>
public enum ErrorType
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5,
    Failure = 6
}