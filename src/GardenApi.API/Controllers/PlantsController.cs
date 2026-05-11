using GardenApi.Application.DTOs.Plant;
using GardenApi.Application.Interfaces;
using GardenApi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlantsController : ControllerBase
{
    private readonly IPlantService _plantService;

    public PlantsController(IPlantService plantService)
    {
        _plantService = plantService;
    }

    /// <summary>
    /// Returns all plants in the database. No authentication required.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PlantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PlantDto>>> GetAll()
    {
        return Ok(await _plantService.GetAllPlantsAsync());
    }

    /// <summary>
    /// Returns a single plant by its ID. No authentication required.
    /// </summary>
    /// <param name="id">The plant ID.</param>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PlantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlantDto>> GetById(int id)
    {
        var plant = await _plantService.GetPlantByIdAsync(id);
        return plant is null ? NotFound() : Ok(plant);
    }

    /// <summary>
    /// Returns all plants of a given type (e.g., Vegetable, Fruit, Herb). No authentication required.
    /// </summary>
    /// <param name="type">The plant type enum value.</param>
    [HttpGet("by-type/{type}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PlantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PlantDto>>> GetByType(PlantType type)
    {
        return Ok(await _plantService.GetPlantsByTypeAsync(type));
    }

    /// <summary>
    /// Returns all plants whose planting zone range contains the given zone number.
    /// For example, zone 8 matches plants with ranges like "6-10" or "7-9". No authentication required.
    /// </summary>
    /// <param name="zone">The USDA hardiness zone number (typically 1–13).</param>
    [HttpGet("by-zone/{zone:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PlantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PlantDto>>> GetByZone(int zone)
    {
        return Ok(await _plantService.GetPlantsByZoneAsync(zone));
    }

    /// <summary>
    /// Returns all plants whose sunlight requirement matches the given level (FullSun, PartialShade, FullShade).
    /// No authentication required.
    /// </summary>
    /// <param name="sunlightLevel">The sunlight level enum value.</param>
    [HttpGet("by-sunlight/{sunlightLevel}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PlantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PlantDto>>> GetBySunlight(SunlightLevel sunlightLevel)
    {
        return Ok(await _plantService.GetCompatiblePlantsForSunlightAsync(sunlightLevel));
    }

    /// <summary>
    /// Creates a new plant. Requires authentication.
    /// </summary>
    /// <param name="request">The plant creation request.</param>
    [HttpPost]
    [ProducesResponseType(typeof(PlantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlantDto>> Create(CreatePlantRequest request)
    {
        var plant = await _plantService.CreatePlantAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = plant.Id }, plant);
    }

    /// <summary>
    /// Updates an existing plant's data. Requires authentication.
    /// </summary>
    /// <param name="id">The plant ID.</param>
    /// <param name="request">The update request.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PlantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlantDto>> Update(int id, UpdatePlantRequest request)
    {
        var plant = await _plantService.UpdatePlantAsync(id, request);
        return plant is null ? NotFound() : Ok(plant);
    }

    /// <summary>
    /// Deletes a plant by ID. Requires authentication.
    /// Plants that are currently assigned to garden items cannot be deleted.
    /// </summary>
    /// <param name="id">The plant ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        return await _plantService.DeletePlantAsync(id) ? NoContent() : NotFound();
    }
}
