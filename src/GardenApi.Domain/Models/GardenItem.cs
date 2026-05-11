using GardenApi.Domain.Enums;

namespace GardenApi.Domain.Models;

public class GardenItem
{
    public int Id { get; set; }
    public int GardenId { get; set; }
    public string? Nickname { get; set; }
    public decimal Height { get; set; }
    public decimal Width { get; set; }
    public GardenItemType Type { get; set; }
    public SunlightLevel SunlightReceived { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public Garden Garden { get; set; } = null!;
    public ICollection<GardenItemPlant> GardenItemPlants { get; set; } = new List<GardenItemPlant>();
}
