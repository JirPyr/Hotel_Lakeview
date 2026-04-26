using HotelLakeview.Application.Interfaces;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using System.Reflection;

namespace HotelLakeview.Infrastructure.Repositories;

/// <summary>
/// In-memory-toteutus huoneiden tallennukseen ja hakemiseen.
/// </summary>
public sealed class InMemoryRoomRepository : IRoomRepository
{
    private static readonly List<Room> Rooms = new();
    private static int _nextId = 1;
    private static bool _isSeeded = false;

    public InMemoryRoomRepository()
    {
        SeedRooms();
    }

    public Task<Room?> GetByIdAsync(int id)
    {
        var room = Rooms.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(room);
    }

    public Task<Room?> GetByRoomNumberAsync(string roomNumber)
    {
        var room = Rooms.FirstOrDefault(r =>
            r.RoomNumber.Equals(roomNumber, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(room);
    }

    public Task<IReadOnlyCollection<Room>> GetPagedAsync(int page, int pageSize)
    {
        var items = Rooms
            .OrderBy(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<Room>)items);
    }

    public Task<int> CountAsync()
    {
        return Task.FromResult(Rooms.Count);
    }

    public Task<IReadOnlyCollection<Room>> GetActiveAsync()
    {
        var items = Rooms
            .Where(r => r.IsActive)
            .OrderBy(r => r.Id)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<Room>)items);
    }

    public Task<IReadOnlyCollection<Room>> GetByRoomTypeAsync(RoomType roomType)
    {
        var items = Rooms
            .Where(r => r.IsActive && r.RoomType == roomType)
            .OrderBy(r => r.Id)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<Room>)items);
    }

    public Task<Room> AddAsync(Room room)
    {
        SetEntityId(room, _nextId++);
        Rooms.Add(room);

        return Task.FromResult(room);
    }

    public Task<Room> UpdateAsync(Room room)
    {
        var existingIndex = Rooms.FindIndex(r => r.Id == room.Id);

        if (existingIndex >= 0)
        {
            Rooms[existingIndex] = room;
        }

        return Task.FromResult(room);
    }

    /// <summary>
    /// Lisää muistivarastoon oletushuoneet vain kerran.
    /// </summary>
private static void SeedRooms()
{
    if (_isSeeded)
    {
        return;
    }

    var seedRooms = new List<Room>
    {
        // ECONOMY
        new Room("101", RoomType.Economy, 1, 79, "Yhden hengen economy-huone. Kompakti ja edullinen vaihtoehto."),
        new Room("102", RoomType.Economy, 1, 79, "Yhden hengen economy-huone rauhallisella sijainnilla."),
        new Room("103", RoomType.Economy, 1, 79, "Perusvarusteltu economy-huone lyhyisiin yöpymisiin."),
        new Room("104", RoomType.Economy, 1, 79, "Yksinkertainen ja siisti economy-huone."),
        new Room("105", RoomType.Economy, 1, 79, "Edullinen huone yhdelle matkustajalle."),
        new Room("106", RoomType.Economy, 1, 79, "Kompakti huone nopeisiin vierailuihin."),
        new Room("107", RoomType.Economy, 1, 79, "Hiljainen economy-huone sisäpihan puolella."),
        new Room("108", RoomType.Economy, 1, 79, "Perushuone yhdelle, kaikki tarpeellinen mukana."),

        // STANDARD
        new Room("201", RoomType.Standard, 2, 119, "Mukava kahden hengen huone, moderni sisustus."),
        new Room("202", RoomType.Standard, 2, 119, "Standard-huone parivuoteella."),
        new Room("203", RoomType.Standard, 2, 119, "Tilava kahden hengen huone."),
        new Room("204", RoomType.Standard, 2, 119, "Huone kahdelle, hyvä sijainti hotellissa."),
        new Room("205", RoomType.Standard, 2, 119, "Parivuoteellinen huone rentouttavaan majoitukseen."),
        new Room("206", RoomType.Standard, 2, 119, "Standard-huone lisätilalla."),
        new Room("207", RoomType.Standard, 2, 119, "Mukava ja valoisa huone kahdelle."),
        new Room("208", RoomType.Standard, 2, 119, "Hyvin varusteltu kahden hengen huone."),
        new Room("209", RoomType.Standard, 2, 119, "Tilava huone kahdelle matkailijalle."),
        new Room("210", RoomType.Standard, 2, 119, "Perinteinen kahden hengen hotellihuone."),

        // SUPERIOR
        new Room("301", RoomType.Superior, 2, 159, "Superior-huone paremmalla varustelulla."),
        new Room("302", RoomType.Superior, 2, 159, "Tilava huone lisämukavuuksilla."),
        new Room("303", RoomType.Superior, 2, 159, "Paranneltu huone modernilla ilmeellä."),
        new Room("304", RoomType.Superior, 2, 159, "Mukavuutta ja tyyliä kahdelle."),
        new Room("305", RoomType.Superior, 2, 159, "Hiljainen superior-huone."),
        new Room("306", RoomType.Superior, 2, 159, "Laadukas huone rauhalliseen oleskeluun."),

        // JUNIOR SUITE
        new Room("401", RoomType.JuniorSuite, 3, 219, "Junior-sviitti lisätilalla ja oleskelualueella."),
        new Room("402", RoomType.JuniorSuite, 3, 219, "Tilava sviitti pienelle seurueelle."),
        new Room("403", RoomType.JuniorSuite, 3, 219, "Mukava sviitti perheelle."),
        new Room("404", RoomType.JuniorSuite, 3, 219, "Valoisa sviitti erillisellä oleskelutilalla."),

        // SUITE
        new Room("501", RoomType.Suite, 4, 319, "Tilava sviitti upeilla näkymillä järvelle."),
        new Room("502", RoomType.Suite, 4, 319, "Tilava sviitti upeilla näkymillä järvelle. Sopii perheille tai romanttiseen lomaan.")
    };

    foreach (var room in seedRooms)
    {
        SetEntityId(room, _nextId++);
        Rooms.Add(room);
    }

    _isSeeded = true;
}

    private static void SetEntityId(Room room, int id)
    {
        var property = typeof(Room).GetProperty(
            nameof(Room.Id),
            BindingFlags.Instance | BindingFlags.Public);

        property?.SetValue(room, id);
    }
}