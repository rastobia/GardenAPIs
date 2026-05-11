using GardenApi.Domain.Enums;
using GardenApi.Domain.Interfaces;
using GardenApi.Domain.Models;
using GardenApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GardenApi.Infrastructure.Repositories;

public class PlantRepository : Repository<Plant>, IPlantRepository
{
    public PlantRepository(GardenDbContext context) : base(context) { }

    public async Task<IEnumerable<Plant>> GetByTypeAsync(PlantType type) =>
        await _context.Plants
            .Where(p => p.Type == type)
            .ToListAsync();

    public async Task<IEnumerable<Plant>> GetByPlantingZoneAsync(int zone)
    {
        var all = await _context.Plants.ToListAsync();
        return all.Where(p => ZoneContains(p.PlantingZone, zone));
    }

    private static bool ZoneContains(string plantingZone, int zone)
    {
        if (string.IsNullOrWhiteSpace(plantingZone))
            return false;

        var parts = plantingZone.Split('-');

        if (parts.Length == 2
            && int.TryParse(parts[0], out var min)
            && int.TryParse(parts[1], out var max))
            return zone >= min && zone <= max;

        if (parts.Length == 1 && int.TryParse(parts[0], out var single))
            return zone == single;

        return false;
    }

    public async Task<IEnumerable<Plant>> GetCompatibleWithSunlightAsync(SunlightLevel sunlightLevel) =>
        await _context.Plants
            .Where(p => p.SunlightRequirement == sunlightLevel)
            .ToListAsync();
}
