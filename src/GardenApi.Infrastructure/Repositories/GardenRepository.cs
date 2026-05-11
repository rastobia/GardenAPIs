using GardenApi.Domain.Interfaces;
using GardenApi.Domain.Models;
using GardenApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GardenApi.Infrastructure.Repositories;

public class GardenRepository : Repository<Garden>, IGardenRepository
{
    public GardenRepository(GardenDbContext context) : base(context) { }

    public async Task<Garden?> GetByIdWithItemsAsync(int id) =>
        await _context.Gardens
            .Include(g => g.GardenItems)
                .ThenInclude(gi => gi.GardenItemPlants)
                    .ThenInclude(gip => gip.Plant)
            .FirstOrDefaultAsync(g => g.Id == id);

    public async Task<IEnumerable<Garden>> GetAllWithItemsAsync() =>
        await _context.Gardens
            .Include(g => g.GardenItems)
                .ThenInclude(gi => gi.GardenItemPlants)
                    .ThenInclude(gip => gip.Plant)
            .ToListAsync();
}
