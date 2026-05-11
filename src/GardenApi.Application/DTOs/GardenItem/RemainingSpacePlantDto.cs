using GardenApi.Application.DTOs.Plant;

namespace GardenApi.Application.DTOs.GardenItem;

public class RemainingSpacePlantDto
{
    public PlantDto Plant { get; set; } = null!;
    public int CanFit { get; set; }
}
