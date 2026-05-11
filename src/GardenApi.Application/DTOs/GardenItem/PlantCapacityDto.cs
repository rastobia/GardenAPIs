namespace GardenApi.Application.DTOs.GardenItem;

public class PlantCapacityDto
{
    public int PlantId { get; set; }
    public string PlantName { get; set; } = string.Empty;
    public decimal PlantSpacing { get; set; }
    public decimal ContainerWidth { get; set; }
    public decimal ContainerHeight { get; set; }
    public decimal TotalArea { get; set; }
    public decimal UsedArea { get; set; }
    public decimal AvailableArea { get; set; }
    public int TotalContainerCapacity { get; set; }
    public int CountInAvailableSpace { get; set; }
}
