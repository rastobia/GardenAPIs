using GardenApi.Application.DTOs.GardenItem;

namespace GardenApi.Application.Interfaces;

public interface IGardenItemService
{
    Task<IEnumerable<GardenItemDto>> GetAllGardenItemsAsync();
    Task<GardenItemDto?> GetGardenItemByIdAsync(int id);
    Task<IEnumerable<GardenItemDto>> GetGardenItemsByGardenAsync(int gardenId);
    Task<GardenItemDto> CreateGardenItemAsync(CreateGardenItemRequest request);
    Task<GardenItemDto?> UpdateGardenItemAsync(int id, UpdateGardenItemRequest request);
    Task<bool> DeleteGardenItemAsync(int id);
    Task<GardenItemDto> AssignPlantAsync(int gardenItemId, int plantId);
    Task<GardenItemDto> RemovePlantAsync(int gardenItemId, int plantId);
    Task<PlantCapacityDto> GetPlantCountForContainerAsync(int gardenItemId, int plantId);
    Task<IEnumerable<RemainingSpacePlantDto>> GetPlantsForRemainingSpaceAsync(int gardenItemId);
}
