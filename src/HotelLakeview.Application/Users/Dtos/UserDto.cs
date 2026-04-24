namespace HotelLakeview.Application.Users.Dtos;

/// <summary>
/// DTO käyttäjän tietojen palauttamiseen.
/// </summary>
public sealed class UserDto
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}