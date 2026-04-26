namespace HotelLakeview.Application.Users.Dtos;

/// <summary>
/// DTO käyttäjän luomiseen.
/// </summary>
public sealed class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}