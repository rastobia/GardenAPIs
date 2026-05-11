using AutoMapper;
using GardenApi.Application.DTOs.Plant;
using GardenApi.Application.Interfaces;
using GardenApi.Domain.Enums;
using GardenApi.Domain.Interfaces;
using GardenApi.Domain.Models;

namespace GardenApi.Application.Services;

public class PlantService : IPlantService
{
    private readonly IPlantRepository _plantRepository;
    private readonly IMapper _mapper;

    public PlantService(IPlantRepository plantRepository, IMapper mapper)
    {
        _plantRepository = plantRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PlantDto>> GetAllPlantsAsync()
    {
        var plants = await _plantRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PlantDto>>(plants);
    }

    public async Task<PlantDto?> GetPlantByIdAsync(int id)
    {
        var plant = await _plantRepository.GetByIdAsync(id);
        return plant is null ? null : _mapper.Map<PlantDto>(plant);
    }

    public async Task<PlantDto> CreatePlantAsync(CreatePlantRequest request)
    {
        var plant = _mapper.Map<Plant>(request);
        var created = await _plantRepository.AddAsync(plant);
        return _mapper.Map<PlantDto>(created);
    }

    public async Task<PlantDto?> UpdatePlantAsync(int id, UpdatePlantRequest request)
    {
        var plant = await _plantRepository.GetByIdAsync(id);
        if (plant is null) return null;

        _mapper.Map(request, plant);
        await _plantRepository.UpdateAsync(plant);
        return _mapper.Map<PlantDto>(plant);
    }

    public async Task<bool> DeletePlantAsync(int id)
    {
        if (!await _plantRepository.ExistsAsync(id)) return false;

        await _plantRepository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<PlantDto>> GetPlantsByTypeAsync(PlantType type)
    {
        var plants = await _plantRepository.GetByTypeAsync(type);
        return _mapper.Map<IEnumerable<PlantDto>>(plants);
    }

    public async Task<IEnumerable<PlantDto>> GetPlantsByZoneAsync(int zone)
    {
        var plants = await _plantRepository.GetByPlantingZoneAsync(zone);
        return _mapper.Map<IEnumerable<PlantDto>>(plants);
    }

    public async Task<IEnumerable<PlantDto>> GetCompatiblePlantsForSunlightAsync(SunlightLevel sunlightLevel)
    {
        var plants = await _plantRepository.GetCompatibleWithSunlightAsync(sunlightLevel);
        return _mapper.Map<IEnumerable<PlantDto>>(plants);
    }
}
