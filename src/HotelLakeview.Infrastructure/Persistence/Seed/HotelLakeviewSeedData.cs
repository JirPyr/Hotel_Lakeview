using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HotelLakeview.Infrastructure.Persistence.Seed;

/// <summary>
/// Vastaa tietokannan alkuperäisestä seed-datasta.
/// </summary>
public static class HotelLakeviewSeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<HotelLakeviewDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("HotelLakeviewSeedData");

        await dbContext.Database.MigrateAsync();

        if (!await dbContext.Rooms.AnyAsync())
        {
            logger.LogInformation("Seeding rooms...");

            var rooms = new List<Room>
            {
                new("101", RoomType.Economy, 1, 79m, "Yhden hengen economy-huone. Kompakti ja edullinen vaihtoehto."),
                new("102", RoomType.Economy, 1, 79m, "Yhden hengen economy-huone rauhallisella sijainnilla."),
                new("103", RoomType.Economy, 1, 79m, "Perusvarusteltu economy-huone lyhyisiin yöpymisiin."),
                new("104", RoomType.Economy, 1, 79m, "Yksinkertainen ja siisti economy-huone."),
                new("105", RoomType.Economy, 1, 79m, "Edullinen huone yhdelle matkustajalle."),
                new("106", RoomType.Economy, 1, 79m, "Kompakti huone nopeisiin vierailuihin."),
                new("107", RoomType.Economy, 1, 79m, "Hiljainen economy-huone sisäpihan puolella."),
                new("108", RoomType.Economy, 1, 79m, "Perushuone yhdelle, kaikki tarpeellinen mukana."),

                new("201", RoomType.Standard, 2, 119m, "Mukava kahden hengen huone, moderni sisustus."),
                new("202", RoomType.Standard, 2, 119m, "Standard-huone parivuoteella."),
                new("203", RoomType.Standard, 2, 119m, "Tilava kahden hengen huone."),
                new("204", RoomType.Standard, 2, 119m, "Huone kahdelle, hyvä sijainti hotellissa."),
                new("205", RoomType.Standard, 2, 119m, "Parivuoteellinen huone rentouttavaan majoitukseen."),
                new("206", RoomType.Standard, 2, 119m, "Standard-huone lisätilalla."),
                new("207", RoomType.Standard, 2, 119m, "Mukava ja valoisa huone kahdelle."),
                new("208", RoomType.Standard, 2, 119m, "Hyvin varusteltu kahden hengen huone."),
                new("209", RoomType.Standard, 2, 119m, "Tilava huone kahdelle matkailijalle."),
                new("210", RoomType.Standard, 2, 119m, "Perinteinen kahden hengen hotellihuone."),

                new("301", RoomType.Superior, 2, 159m, "Superior-huone paremmalla varustelulla."),
                new("302", RoomType.Superior, 2, 159m, "Tilava huone lisämukavuuksilla."),
                new("303", RoomType.Superior, 2, 159m, "Paranneltu huone modernilla ilmeellä."),
                new("304", RoomType.Superior, 2, 159m, "Mukavuutta ja tyyliä kahdelle."),
                new("305", RoomType.Superior, 2, 159m, "Hiljainen superior-huone."),
                new("306", RoomType.Superior, 2, 159m, "Laadukas huone rauhalliseen oleskeluun."),

                new("401", RoomType.JuniorSuite, 3, 219m, "Junior-sviitti lisätilalla ja oleskelualueella."),
                new("402", RoomType.JuniorSuite, 3, 219m, "Tilava sviitti pienelle seurueelle."),
                new("403", RoomType.JuniorSuite, 3, 219m, "Mukava sviitti perheelle."),
                new("404", RoomType.JuniorSuite, 3, 219m, "Valoisa sviitti erillisellä oleskelutilalla."),

                new("501", RoomType.Suite, 4, 319m, "Tilava sviitti upeilla näkymillä järvelle."),
                new("502", RoomType.Suite, 4, 319m, "Tilava sviitti upeilla näkymillä järvelle. Sopii perheille tai romanttiseen lomaan.")
            };

            await dbContext.Rooms.AddRangeAsync(rooms);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Customers.AnyAsync())
        {
            logger.LogInformation("Seeding customers...");

            var customers = new List<Customer>
            {
                new("Liisa Järvinen", "liisa.jarvinen@example.com", "0401000001", "Pähkinäallergia"),
                new("Matti Meikäläinen", "matti.meikalainen@example.com", "0401000002", "Myöhäinen saapuminen"),
                new("Anna Virtanen", "anna.virtanen@example.com", "0401000003", "Hiljainen huone"),
                new("Pekka Nieminen", "pekka.nieminen@example.com", "0401000004", "Lisätyyny"),
                new("Sari Lahtinen", "sari.lahtinen@example.com", "0401000005", "Gluteeniton aamiainen")
            };

            await dbContext.Customers.AddRangeAsync(customers);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Users.AnyAsync())
        {
            logger.LogInformation("Seeding users...");

            var users = new List<User>
            {
                new("reception", "TEMP_HASH_CHANGE_ME", UserRole.Receptionist),
                new("management", "TEMP_HASH_CHANGE_ME", UserRole.Management)
            };

            await dbContext.Users.AddRangeAsync(users);
            await dbContext.SaveChangesAsync();
        }
    }
}