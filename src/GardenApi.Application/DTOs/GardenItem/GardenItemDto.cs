namespace GardenApi.Application.DTOs.GardenItem;

public class GardenItemDto
{
    public int Id { get; set; }
    public int GardenId { get; set; }
    public string? Nickname { get; set; }
    public decimal Height { get; set; }
    public decimal Width { get; set; }
    public string Type { get; set; } = string.Empty;
    public string SunlightReceived { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public IEnumerable<PlantSummaryDto> Plants { get; set; } = new List<PlantSummaryDto>();
    public decimal TotalArea { get; set; }
    public decimal UsedArea { get; set; }
    public decimal AvailableArea { get; set; }
}
