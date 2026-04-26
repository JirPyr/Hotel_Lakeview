namespace HotelLakeview.Application.Common.Results;

/// <summary>
/// Kuvaa operaation tuloksen ilman palautettavaa dataa.
/// </summary>
public class Result
{
    /// <summary>
    /// Luo uuden tuloksen.
    /// </summary>
    /// <param name="isSuccess">Kertoo onnistuiko operaatio.</param>
    /// <param name="error">Operaatioon liittyvä virhe.</param>
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error.Type != ErrorType.None)
        {
            throw new ArgumentException("Successful result cannot contain an error.", nameof(error));
        }

        if (!isSuccess && error.Type == ErrorType.None)
        {
            throw new ArgumentException("Failed result must contain an error.", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Kertoo onnistuiko operaatio.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Kertoo epäonnistuiko operaatio.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Virhetieto epäonnistuneessa tilanteessa.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Luo onnistuneen tuloksen.
    /// </summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Luo epäonnistuneen tuloksen.
    /// </summary>
    /// <param name="error">Virhetieto.</param>
    public static Result Failure(Error error) => new(false, error);
}