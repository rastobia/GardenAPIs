using AutoMapper;
using GardenApi.Application.DTOs.Garden;
using GardenApi.Application.DTOs.GardenItem;
using GardenApi.Application.DTOs.Plant;
using GardenApi.Domain.Models;

namespace GardenApi.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Garden
        CreateMap<Garden, GardenDto>()
            .ForMember(d => d.GardenItemCount, o => o.MapFrom(s => s.GardenItems.Count));
        CreateMap<CreateGardenRequest, Garden>();
        CreateMap<UpdateGardenRequest, Garden>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedDate, o => o.Ignore())
            .ForMember(d => d.ModifiedDate, o => o.Ignore())
            .ForMember(d => d.GardenItems, o => o.Ignore());

        // Plant
        CreateMap<Plant, PlantDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.SunlightRequirement, o => o.MapFrom(s => s.SunlightRequirement.ToString()));
        CreateMap<CreatePlantRequest, Plant>();
        CreateMap<UpdatePlantRequest, Plant>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.GardenItemPlants, o => o.Ignore());

        // GardenItem
        CreateMap<GardenItem, GardenItemDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.SunlightReceived, o => o.MapFrom(s => s.SunlightReceived.ToString()))
            .ForMember(d => d.Plants, o => o.MapFrom(s => s.GardenItemPlants.Select(gip => gip.Plant)))
            .ForMember(d => d.TotalArea, o => o.MapFrom(s => s.Width * s.Height))
            .ForMember(d => d.UsedArea, o => o.MapFrom(s =>
                s.GardenItemPlants.Sum(gip => gip.Plant.Spacing * gip.Plant.Spacing)))
            .ForMember(d => d.AvailableArea, o => o.MapFrom(s =>
                s.Width * s.Height - s.GardenItemPlants.Sum(gip => gip.Plant.Spacing * gip.Plant.Spacing)));
        CreateMap<Plant, PlantSummaryDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.SunlightRequirement, o => o.MapFrom(s => s.SunlightRequirement.ToString()));
        CreateMap<CreateGardenItemRequest, GardenItem>();
        CreateMap<UpdateGardenItemRequest, GardenItem>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.GardenId, o => o.Ignore())
            .ForMember(d => d.GardenItemPlants, o => o.Ignore())
            .ForMember(d => d.CreatedDate, o => o.Ignore())
            .ForMember(d => d.UpdatedDate, o => o.Ignore())
            .ForMember(d => d.Garden, o => o.Ignore());
    }
}
