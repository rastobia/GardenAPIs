using GardenApi.Application.DTOs.Plant;
using GardenApi.Domain.Enums;

namespace GardenApi.Application.Interfaces;

public interface IPlantService
{
    Task<IEnumerable<PlantDto>> GetAllPlantsAsync();
    Task<PlantDto?> GetPlantByIdAsync(int id);
    Task<PlantDto> CreatePlantAsync(CreatePlantRequest request);
    Task<PlantDto?> UpdatePlantAsync(int id, UpdatePlantRequest request);
    Task<bool> DeletePlantAsync(int id);
    Task<IEnumerable<PlantDto>> GetPlantsByTypeAsync(PlantType type);
    Task<IEnumerable<PlantDto>> GetPlantsByZoneAsync(int zone);
    Task<IEnumerable<PlantDto>> GetCompatiblePlantsForSunlightAsync(SunlightLevel sunlightLevel);
}
