using GardenApi.Domain.Models;

namespace GardenApi.Domain.Interfaces;

public interface IGardenItemRepository : IRepository<GardenItem>
{
    Task<IEnumerable<GardenItem>> GetAllWithPlantsAsync();
    Task<IEnumerable<GardenItem>> GetByGardenIdAsync(int gardenId);
    Task<GardenItem?> GetByIdWithPlantsAsync(int id);
    Task<GardenItemPlant?> GetGardenItemPlantAsync(int gardenItemId, int plantId);
    Task RemoveGardenItemPlantAsync(GardenItemPlant entry);
}
