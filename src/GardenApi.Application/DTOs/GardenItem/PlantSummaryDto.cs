namespace GardenApi.Application.DTOs.GardenItem;

public class PlantSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string SunlightRequirement { get; set; } = string.Empty;
}
