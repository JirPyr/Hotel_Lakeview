namespace HotelLakeview.Application.Auth.Dtos;

/// <summary>
/// DTO kirjautumisvastauksen palauttamiseen.
/// </summary>
public sealed class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}