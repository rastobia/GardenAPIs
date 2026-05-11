using GardenApi.Application.DTOs.Garden;

namespace GardenApi.Application.Interfaces;

public interface IGardenService
{
    Task<IEnumerable<GardenDto>> GetAllGardensAsync();
    Task<GardenDto?> GetGardenByIdAsync(int id);
    Task<GardenDto> CreateGardenAsync(CreateGardenRequest request);
    Task<GardenDto?> UpdateGardenAsync(int id, UpdateGardenRequest request);
    Task<bool> DeleteGardenAsync(int id);
}
