using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelLakeview.Infrastructure.Persistence;

/// <summary>
/// Design time -factory EF Core migrationeita varten.
/// </summary>
public sealed class HotelLakeviewDbContextFactory
    : IDesignTimeDbContextFactory<HotelLakeviewDbContext>
{
    public HotelLakeviewDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HotelLakeviewDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=HotelLakeviewDb;Username=postgres;Password=postgres");

        return new HotelLakeviewDbContext(optionsBuilder.Options);
    }
}