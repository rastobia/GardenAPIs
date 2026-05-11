using GardenApi.Domain.Enums;

namespace GardenApi.Domain.Models;

public class Plant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PlantType Type { get; set; }
    public SunlightLevel SunlightRequirement { get; set; }
    public int WaterFrequencyDays { get; set; }
    public bool Annual { get; set; }
    public string PlantingZone { get; set; } = string.Empty;
    public decimal Spacing { get; set; }
    public ICollection<GardenItemPlant> GardenItemPlants { get; set; } = new List<GardenItemPlant>();
}
