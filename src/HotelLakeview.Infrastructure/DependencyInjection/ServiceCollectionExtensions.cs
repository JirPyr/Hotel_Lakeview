using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Infrastructure.Persistence;
using HotelLakeview.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HotelLakeview.Infrastructure.Repositories.Ef;

namespace HotelLakeview.Infrastructure.DependencyInjection;

/// <summary>
/// Infrastructure-kerroksen palvelurekisteröinnit.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<HotelLakeviewDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Pidä vielä InMemory käytössä, kunnes EF-repositoryt on toteutettu.
        //päivitetty EF-repositoryt käyttöön
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository    >();
        services.AddScoped<IRoomImageRepository, RoomImageRepository>();

        return services;
    }
}