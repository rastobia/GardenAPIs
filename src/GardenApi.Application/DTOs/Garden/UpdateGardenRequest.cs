using System.ComponentModel.DataAnnotations;

namespace GardenApi.Application.DTOs.Garden;

public class UpdateGardenRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
