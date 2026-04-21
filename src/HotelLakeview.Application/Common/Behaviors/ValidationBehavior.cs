using FluentValidation;
using HotelLakeview.Application.Common.Results;
using MediatR;
using System.Reflection;

namespace HotelLakeview.Application.Common.Behaviors;

/// <summary>
/// Suorittaa requestin FluentValidation-validaattorit ennen handleria.
/// </summary>
/// <typeparam name="TRequest">Käsiteltävän pyynnön tyyppi.</typeparam>
/// <typeparam name="TResponse">Palautettavan vastauksen tyyppi.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Luo uuden validointibehaviorin.
    /// </summary>
    /// <param name="validators">Pyynnölle rekisteröidyt validaattorit.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Suorittaa validaattorit ennen varsinaista handleria.
    /// </summary>
    /// <param name="request">Käsiteltävä pyyntö.</param>
    /// <param name="next">Seuraava vaihe pipeline:ssa.</param>
    /// <param name="cancellationToken">Peruutustunniste.</param>
    /// <returns>Handlerin vastaus tai validointivirhe.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        var errorMessages = string.Join("; ", failures.Select(f => f.ErrorMessage));
        var error = Error.Validation("ValidationError", errorMessages);

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];
            var resultType = typeof(Result<>).MakeGenericType(valueType);

            var failureMethod = resultType.GetMethod(
                nameof(Result<object>.Failure),
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: new[] { typeof(Error) },
                modifiers: null);

            if (failureMethod is null)
            {
                throw new InvalidOperationException(
                    $"Failure-metodia ei löytynyt tyypille {resultType.Name}.");
            }

            var failureResult = failureMethod.Invoke(null, new object[] { error });

            return (TResponse)failureResult!;
        }

        throw new InvalidOperationException(
            $"ValidationBehavior ei tue response-tyyppiä {typeof(TResponse).Name}.");
    }
}