using HotelLakeview.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Persistence;

/// <summary>
/// Sovelluksen EF Core -tietokantakonteksti.
/// Vastaa entiteettien liittämisestä tietokantaan.
/// </summary>
public sealed class HotelLakeviewDbContext : DbContext
{
    /// <summary>
    /// Luo uuden tietokantakontekstin.
    /// </summary>
    /// <param name="options">EF Coren konfiguroimat asetukset.</param>
    public HotelLakeviewDbContext(DbContextOptions<HotelLakeviewDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Asiakkaat.
    /// </summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>
    /// Huoneet.
    /// </summary>
    public DbSet<Room> Rooms => Set<Room>();

    /// <summary>
    /// Varaukset.
    /// </summary>
    public DbSet<Reservation> Reservations => Set<Reservation>();

    /// <summary>
    /// Huonekuvat.
    /// </summary>
    public DbSet<RoomImage> RoomImages => Set<RoomImage>();

    /// <summary>
    /// Järjestelmän käyttäjät.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Lataa kaikki Fluent API -konfiguraatiot samasta assemblystä.
    /// </summary>
    /// <param name="modelBuilder">EF-mallin rakentaja.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HotelLakeviewDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}