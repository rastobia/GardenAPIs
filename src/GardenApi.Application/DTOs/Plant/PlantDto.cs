namespace GardenApi.Application.DTOs.Plant;

public class PlantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string SunlightRequirement { get; set; } = string.Empty;
    public int WaterFrequencyDays { get; set; }
    public bool Annual { get; set; }
    public string PlantingZone { get; set; } = string.Empty;
    public decimal Spacing { get; set; }
}
