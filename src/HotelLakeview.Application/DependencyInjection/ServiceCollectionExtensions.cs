using FluentValidation;
using HotelLakeview.Application.Common.Behaviors;
using HotelLakeview.Application.Reservations.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HotelLakeview.Application.DependencyInjection;

/// <summary>
/// Application-kerroksen palvelurekisteröinnit.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Rekisteröi Application-kerroksen handlerit, validaattorit ja pipeline behaviorit.
    /// </summary>
    /// <param name="services">Palvelukokoelma.</param>
    /// <returns>Päivitetty palvelukokoelma.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IReservationPricingService, ReservationPricingService>();

        return services;
    }
}