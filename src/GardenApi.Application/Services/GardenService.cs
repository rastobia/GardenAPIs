using AutoMapper;
using GardenApi.Application.DTOs.Garden;
using GardenApi.Application.Interfaces;
using GardenApi.Domain.Interfaces;
using GardenApi.Domain.Models;

namespace GardenApi.Application.Services;

public class GardenService : IGardenService
{
    private readonly IGardenRepository _gardenRepository;
    private readonly IMapper _mapper;

    public GardenService(IGardenRepository gardenRepository, IMapper mapper)
    {
        _gardenRepository = gardenRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GardenDto>> GetAllGardensAsync()
    {
        var gardens = await _gardenRepository.GetAllWithItemsAsync();
        return _mapper.Map<IEnumerable<GardenDto>>(gardens);
    }

    public async Task<GardenDto?> GetGardenByIdAsync(int id)
    {
        var garden = await _gardenRepository.GetByIdWithItemsAsync(id);
        return garden is null ? null : _mapper.Map<GardenDto>(garden);
    }

    public async Task<GardenDto> CreateGardenAsync(CreateGardenRequest request)
    {
        var garden = _mapper.Map<Garden>(request);
        garden.CreatedDate = DateTime.UtcNow;
        garden.ModifiedDate = DateTime.UtcNow;

        var created = await _gardenRepository.AddAsync(garden);
        return _mapper.Map<GardenDto>(created);
    }

    public async Task<GardenDto?> UpdateGardenAsync(int id, UpdateGardenRequest request)
    {
        var garden = await _gardenRepository.GetByIdAsync(id);
        if (garden is null) return null;

        _mapper.Map(request, garden);
        garden.ModifiedDate = DateTime.UtcNow;
        await _gardenRepository.UpdateAsync(garden);

        return _mapper.Map<GardenDto>(garden);
    }

    public async Task<bool> DeleteGardenAsync(int id)
    {
        if (!await _gardenRepository.ExistsAsync(id)) return false;

        await _gardenRepository.DeleteAsync(id);
        return true;
    }
}
