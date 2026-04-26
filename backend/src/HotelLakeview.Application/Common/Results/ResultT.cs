namespace HotelLakeview.Application.Common.Results;

/// <summary>
/// Kuvaa operaation tuloksen palautettavan datan kanssa.
/// </summary>
/// <typeparam name="T">Palautettavan datan tyyppi.</typeparam>
public class Result<T> : Result
{
    private readonly T? _value;

    /// <summary>
    /// Luo uuden geneerisen tuloksen.
    /// </summary>
    /// <param name="value">Palautettava arvo onnistuneessa tilanteessa.</param>
    /// <param name="isSuccess">Kertoo onnistuiko operaatio.</param>
    /// <param name="error">Virhetieto.</param>
    protected Result(T? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Palautettava arvo onnistuneessa tilanteessa.
    /// </summary>
    /// <exception cref="InvalidOperationException">Heitetään, jos arvoa yritetään lukea epäonnistuneesta tuloksesta.</exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Failed result does not contain a value.");

    /// <summary>
    /// Luo onnistuneen tuloksen datan kanssa.
    /// </summary>
    /// <param name="value">Palautettava arvo.</param>
    public static Result<T> Success(T value) => new(value, true, Error.None);

    /// <summary>
    /// Luo epäonnistuneen tuloksen.
    /// </summary>
    /// <param name="error">Virhetieto.</param>
    public new static Result<T> Failure(Error error) => new(default, false, error);
}