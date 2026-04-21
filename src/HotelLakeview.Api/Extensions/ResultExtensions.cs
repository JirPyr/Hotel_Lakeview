using HotelLakeview.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Extensions;

/// <summary>
/// Extension-metodit Result-olioiden muuntamiseen HTTP-vastauksiksi.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Muuntaa Result-olion IActionResult-muotoon.
    /// </summary>
    /// <param name="result">Muunnettava tulos.</param>
    /// <param name="controller">Nykyinen controller.</param>
    /// <returns>HTTP-vastaus Result-olion tilan perusteella.</returns>
    public static IActionResult ToActionResult(
        this Result result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return result.Error.Type switch
        {
            ErrorType.Validation => controller.BadRequest(result.Error),
            ErrorType.NotFound => controller.NotFound(result.Error),
            ErrorType.Conflict => controller.Conflict(result.Error),
            ErrorType.Unauthorized => controller.Unauthorized(result.Error),
            _ => controller.BadRequest(result.Error)
        };
    }

    /// <summary>
    /// Muuntaa geneerisen Result-olion IActionResult-muotoon.
    /// </summary>
    /// <typeparam name="T">Palautettavan datan tyyppi.</typeparam>
    /// <param name="result">Muunnettava tulos.</param>
    /// <param name="controller">Nykyinen controller.</param>
    /// <returns>HTTP-vastaus Result-olion tilan perusteella.</returns>
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return result.Error.Type switch
        {
            ErrorType.Validation => controller.BadRequest(result.Error),
            ErrorType.NotFound => controller.NotFound(result.Error),
            ErrorType.Conflict => controller.Conflict(result.Error),
            ErrorType.Unauthorized => controller.Unauthorized(result.Error),
            _ => controller.BadRequest(result.Error)
        };
    }
}