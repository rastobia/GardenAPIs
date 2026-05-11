using GardenApi.Domain.Enums;
using GardenApi.Domain.Models;

namespace GardenApi.Domain.Interfaces;

public interface IPlantRepository : IRepository<Plant>
{
    Task<IEnumerable<Plant>> GetByTypeAsync(PlantType type);
    Task<IEnumerable<Plant>> GetByPlantingZoneAsync(int zone);
    Task<IEnumerable<Plant>> GetCompatibleWithSunlightAsync(SunlightLevel sunlightLevel);
}
