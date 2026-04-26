namespace HotelLakeview.Application.Common.Results;

/// <summary>
/// Kuvaa yksittäisen sovellusvirheen.
/// </summary>
public sealed class Error
{
    /// <summary>
    /// Luo uuden virheen.
    /// </summary>
    /// <param name="code">Virhekoodi.</param>
    /// <param name="message">Virheen kuvaus.</param>
    /// <param name="type">Virheen tyyppi.</param>
    public Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    /// <summary>
    /// Virhekoodi, jota voidaan käyttää ohjelmallisesti.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Ihmiselle luettava virheviesti.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Virheen luokittelu.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Tyhjä virhe onnistuneita palautuksia varten.
    /// </summary>
    public static Error None => new(string.Empty, string.Empty, ErrorType.None);

    /// <summary>
    /// Luo validointivirheen.
    /// </summary>
    public static Error Validation(string code, string message)
        => new(code, message, ErrorType.Validation);

    /// <summary>
    /// Luo not found -virheen.
    /// </summary>
    public static Error NotFound(string code, string message)
        => new(code, message, ErrorType.NotFound);

    /// <summary>
    /// Luo conflict-virheen.
    /// </summary>
    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);

    /// <summary>
    /// Luo unauthorized-virheen.
    /// </summary>
    public static Error Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);

    /// <summary>
    /// Luo forbidden-virheen.
    /// </summary>
    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);

    /// <summary>
    /// Luo yleisen virheen.
    /// </summary>
    public static Error Failure(string code, string message)
        => new(code, message, ErrorType.Failure);
}