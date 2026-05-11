namespace GardenApi.Domain.Models;

public class Garden
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public ICollection<GardenItem> GardenItems { get; set; } = new List<GardenItem>();
}
