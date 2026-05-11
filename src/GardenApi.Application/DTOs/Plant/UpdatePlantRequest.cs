using System.ComponentModel.DataAnnotations;
using GardenApi.Domain.Enums;

namespace GardenApi.Application.DTOs.Plant;

public class UpdatePlantRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public PlantType Type { get; set; }

    [Required]
    public SunlightLevel SunlightRequirement { get; set; }

    [Range(1, 365)]
    public int WaterFrequencyDays { get; set; }

    public bool Annual { get; set; }

    [Required]
    [MaxLength(10)]
    public string PlantingZone { get; set; } = string.Empty;

    [Range(0.1, 9999.9)]
    public decimal Spacing { get; set; }
}
