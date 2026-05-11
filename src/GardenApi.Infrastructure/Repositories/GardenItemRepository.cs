using GardenApi.Domain.Interfaces;
using GardenApi.Domain.Models;
using GardenApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GardenApi.Infrastructure.Repositories;

public class GardenItemRepository : Repository<GardenItem>, IGardenItemRepository
{
    public GardenItemRepository(GardenDbContext context) : base(context) { }

    public async Task<IEnumerable<GardenItem>> GetAllWithPlantsAsync() =>
        await _context.GardenItems
            .Include(gi => gi.GardenItemPlants)
                .ThenInclude(gip => gip.Plant)
            .ToListAsync();

    public async Task<IEnumerable<GardenItem>> GetByGardenIdAsync(int gardenId) =>
        await _context.GardenItems
            .Include(gi => gi.GardenItemPlants)
                .ThenInclude(gip => gip.Plant)
            .Where(gi => gi.GardenId == gardenId)
            .ToListAsync();

    public async Task<GardenItem?> GetByIdWithPlantsAsync(int id) =>
        await _context.GardenItems
            .Include(gi => gi.GardenItemPlants)
                .ThenInclude(gip => gip.Plant)
            .FirstOrDefaultAsync(gi => gi.Id == id);

    public async Task<GardenItemPlant?> GetGardenItemPlantAsync(int gardenItemId, int plantId) =>
        await _context.GardenItemPlants
            .FirstOrDefaultAsync(gip => gip.GardenItemId == gardenItemId && gip.PlantId == plantId);

    public async Task RemoveGardenItemPlantAsync(GardenItemPlant entry)
    {
        _context.GardenItemPlants.Remove(entry);
        await _context.SaveChangesAsync();
    }
}
