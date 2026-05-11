namespace GardenApi.Application.DTOs.Garden;

public class GardenDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public int GardenItemCount { get; set; }
}
