using AutoMapper;
using GardenApi.Application.DTOs.GardenItem;
using GardenApi.Application.DTOs.Plant;
using GardenApi.Application.Interfaces;
using GardenApi.Domain.Interfaces;
using GardenApi.Domain.Models;

namespace GardenApi.Application.Services;

public class GardenItemService : IGardenItemService
{
    private readonly IGardenItemRepository _gardenItemRepository;
    private readonly IPlantRepository _plantRepository;
    private readonly IGardenRepository _gardenRepository;
    private readonly IMapper _mapper;

    public GardenItemService(
        IGardenItemRepository gardenItemRepository,
        IPlantRepository plantRepository,
        IGardenRepository gardenRepository,
        IMapper mapper)
    {
        _gardenItemRepository = gardenItemRepository;
        _plantRepository = plantRepository;
        _gardenRepository = gardenRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GardenItemDto>> GetAllGardenItemsAsync()
    {
        var items = await _gardenItemRepository.GetAllWithPlantsAsync();
        return _mapper.Map<IEnumerable<GardenItemDto>>(items);
    }

    public async Task<GardenItemDto?> GetGardenItemByIdAsync(int id)
    {
        var item = await _gardenItemRepository.GetByIdWithPlantsAsync(id);
        return item is null ? null : _mapper.Map<GardenItemDto>(item);
    }

    public async Task<IEnumerable<GardenItemDto>> GetGardenItemsByGardenAsync(int gardenId)
    {
        var items = await _gardenItemRepository.GetByGardenIdAsync(gardenId);
        return _mapper.Map<IEnumerable<GardenItemDto>>(items);
    }

    public async Task<GardenItemDto> CreateGardenItemAsync(CreateGardenItemRequest request)
    {
        if (!await _gardenRepository.ExistsAsync(request.GardenId))
            throw new KeyNotFoundException($"Garden with id {request.GardenId} not found.");

        var item = _mapper.Map<GardenItem>(request);
        item.CreatedDate = DateTime.UtcNow;
        item.UpdatedDate = DateTime.UtcNow;

        var created = await _gardenItemRepository.AddAsync(item);
        return _mapper.Map<GardenItemDto>(created);
    }

    public async Task<GardenItemDto?> UpdateGardenItemAsync(int id, UpdateGardenItemRequest request)
    {
        var item = await _gardenItemRepository.GetByIdAsync(id);
        if (item is null) return null;

        _mapper.Map(request, item);
        item.UpdatedDate = DateTime.UtcNow;
        await _gardenItemRepository.UpdateAsync(item);

        return _mapper.Map<GardenItemDto>(await _gardenItemRepository.GetByIdWithPlantsAsync(id));
    }

    public async Task<bool> DeleteGardenItemAsync(int id)
    {
        if (!await _gardenItemRepository.ExistsAsync(id)) return false;

        await _gardenItemRepository.DeleteAsync(id);
        return true;
    }

    public async Task<GardenItemDto> AssignPlantAsync(int gardenItemId, int plantId)
    {
        var gardenItem = await _gardenItemRepository.GetByIdWithPlantsAsync(gardenItemId)
            ?? throw new KeyNotFoundException($"GardenItem with id {gardenItemId} not found.");

        var plant = await _plantRepository.GetByIdAsync(plantId)
            ?? throw new KeyNotFoundException($"Plant with id {plantId} not found.");

        if (plant.SunlightRequirement != gardenItem.SunlightReceived)
            throw new InvalidOperationException(
                $"Sunlight mismatch: '{plant.Name}' requires {plant.SunlightRequirement} " +
                $"but this garden item receives {gardenItem.SunlightReceived}.");

        if (plant.Spacing > gardenItem.Width || plant.Spacing > gardenItem.Height)
            throw new InvalidOperationException(
                $"Not enough space: '{plant.Name}' needs {plant.Spacing}\" spacing but this container " +
                $"is only {gardenItem.Width}\"×{gardenItem.Height}\". " +
                $"Minimum container size required: {plant.Spacing}\"×{plant.Spacing}\".");

        var usedArea = gardenItem.GardenItemPlants.Sum(gip => gip.Plant.Spacing * gip.Plant.Spacing);
        var availableArea = gardenItem.Width * gardenItem.Height - usedArea;
        var plantArea = plant.Spacing * plant.Spacing;

        if (plantArea > availableArea)
            throw new InvalidOperationException(
                $"Not enough space: '{plant.Name}' requires {plantArea} sq\" but only {availableArea} sq\" " +
                $"remains in this container after accounting for {gardenItem.GardenItemPlants.Count} existing plant(s).");

        gardenItem.GardenItemPlants.Add(new GardenItemPlant
        {
            PlantId = plantId,
            AddedDate = DateTime.UtcNow
        });
        gardenItem.UpdatedDate = DateTime.UtcNow;
        await _gardenItemRepository.UpdateAsync(gardenItem);

        return _mapper.Map<GardenItemDto>(await _gardenItemRepository.GetByIdWithPlantsAsync(gardenItemId));
    }

    public async Task<GardenItemDto> RemovePlantAsync(int gardenItemId, int plantId)
    {
        var gardenItem = await _gardenItemRepository.GetByIdWithPlantsAsync(gardenItemId)
            ?? throw new KeyNotFoundException($"GardenItem with id {gardenItemId} not found.");

        var entry = await _gardenItemRepository.GetGardenItemPlantAsync(gardenItemId, plantId)
            ?? throw new KeyNotFoundException($"Plant with id {plantId} is not assigned to GardenItem {gardenItemId}.");

        await _gardenItemRepository.RemoveGardenItemPlantAsync(entry);

        gardenItem.UpdatedDate = DateTime.UtcNow;
        await _gardenItemRepository.UpdateAsync(gardenItem);

        return _mapper.Map<GardenItemDto>(await _gardenItemRepository.GetByIdWithPlantsAsync(gardenItemId));
    }

    public async Task<PlantCapacityDto> GetPlantCountForContainerAsync(int gardenItemId, int plantId)
    {
        var gardenItem = await _gardenItemRepository.GetByIdWithPlantsAsync(gardenItemId)
            ?? throw new KeyNotFoundException($"GardenItem with id {gardenItemId} not found.");

        var plant = await _plantRepository.GetByIdAsync(plantId)
            ?? throw new KeyNotFoundException($"Plant with id {plantId} not found.");

        var totalArea = gardenItem.Width * gardenItem.Height;
        var usedArea = gardenItem.GardenItemPlants.Sum(gip => gip.Plant.Spacing * gip.Plant.Spacing);
        var availableArea = totalArea - usedArea;
        var plantArea = plant.Spacing * plant.Spacing;

        return new PlantCapacityDto
        {
            PlantId = plant.Id,
            PlantName = plant.Name,
            PlantSpacing = plant.Spacing,
            ContainerWidth = gardenItem.Width,
            ContainerHeight = gardenItem.Height,
            TotalArea = totalArea,
            UsedArea = usedArea,
            AvailableArea = availableArea,
            TotalContainerCapacity = (int)(gardenItem.Width / plant.Spacing) * (int)(gardenItem.Height / plant.Spacing),
            CountInAvailableSpace = plantArea > 0 ? (int)(availableArea / plantArea) : 0
        };
    }

    public async Task<IEnumerable<RemainingSpacePlantDto>> GetPlantsForRemainingSpaceAsync(int gardenItemId)
    {
        var gardenItem = await _gardenItemRepository.GetByIdWithPlantsAsync(gardenItemId)
            ?? throw new KeyNotFoundException($"GardenItem with id {gardenItemId} not found.");

        var usedArea = gardenItem.GardenItemPlants.Sum(gip => gip.Plant.Spacing * gip.Plant.Spacing);
        var availableArea = gardenItem.Width * gardenItem.Height - usedArea;

        var allPlants = await _plantRepository.GetAllAsync();

        return allPlants
            .Where(p => p.Spacing <= gardenItem.Width
                     && p.Spacing <= gardenItem.Height
                     && p.Spacing * p.Spacing <= availableArea)
            .Select(p => new RemainingSpacePlantDto
            {
                Plant = _mapper.Map<PlantDto>(p),
                CanFit = (int)(availableArea / (p.Spacing * p.Spacing))
            })
            .OrderByDescending(r => r.CanFit)
            .ToList();
    }
}
