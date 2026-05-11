namespace GardenApi.Domain.Models;

public class GardenItemPlant
{
    public int Id { get; set; }
    public int GardenItemId { get; set; }
    public int PlantId { get; set; }
    public DateTime AddedDate { get; set; }
    public GardenItem GardenItem { get; set; } = null!;
    public Plant Plant { get; set; } = null!;
}
