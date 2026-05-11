using System.ComponentModel.DataAnnotations;
using GardenApi.Domain.Enums;

namespace GardenApi.Application.DTOs.GardenItem;

public class UpdateGardenItemRequest
{
    [MaxLength(100)]
    public string? Nickname { get; set; }

    [Range(0.01, 9999.99)]
    public decimal Height { get; set; }

    [Range(0.01, 9999.99)]
    public decimal Width { get; set; }

    [Required]
    public GardenItemType Type { get; set; }

    [Required]
    public SunlightLevel SunlightReceived { get; set; }
}
