using GardenApi.Application.DTOs.GardenItem;
using GardenApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GardenItemsController : ControllerBase
{
    private readonly IGardenItemService _gardenItemService;

    public GardenItemsController(IGardenItemService gardenItemService)
    {
        _gardenItemService = gardenItemService;
    }

    /// <summary>
    /// Returns all garden items with their assigned plants and area calculations.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GardenItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GardenItemDto>>> GetAll()
    {
        return Ok(await _gardenItemService.GetAllGardenItemsAsync());
    }

    /// <summary>
    /// Returns a single garden item by its ID, including assigned plants and area data.
    /// </summary>
    /// <param name="id">The garden item ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GardenItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GardenItemDto>> GetById(int id)
    {
        var item = await _gardenItemService.GetGardenItemByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Returns all garden items that belong to a specific garden.
    /// </summary>
    /// <param name="gardenId">The garden ID to filter by.</param>
    [HttpGet("by-garden/{gardenId:int}")]
    [ProducesResponseType(typeof(IEnumerable<GardenItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GardenItemDto>>> GetByGarden(int gardenId)
    {
        return Ok(await _gardenItemService.GetGardenItemsByGardenAsync(gardenId));
    }

    /// <summary>
    /// Calculates how many of a given plant can fit in a garden item, accounting for plants already assigned.
    /// Returns total container capacity, currently used area, and how many more of this plant fit in the remaining space.
    /// </summary>
    /// <param name="id">The garden item ID.</param>
    /// <param name="plantId">The plant ID to calculate capacity for.</param>
    [HttpGet("{id:int}/plant-capacity/{plantId:int}")]
    [ProducesResponseType(typeof(PlantCapacityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlantCapacityDto>> GetPlantCapacity(int id, int plantId)
    {
        try
        {
            return Ok(await _gardenItemService.GetPlantCountForContainerAsync(id, plantId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Returns all plants that can fit in the remaining available space of a garden item.
    /// Results are ordered by how many of each plant could fit, descending.
    /// </summary>
    /// <param name="id">The garden item ID.</param>
    [HttpGet("{id:int}/remaining-space")]
    [ProducesResponseType(typeof(IEnumerable<RemainingSpacePlantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<RemainingSpacePlantDto>>> GetRemainingSpacePlants(int id)
    {
        try
        {
            return Ok(await _gardenItemService.GetPlantsForRemainingSpaceAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new garden item (container) within an existing garden.
    /// Specify the container's dimensions (Width × Height in inches), type, sunlight, and an optional nickname.
    /// </summary>
    /// <param name="request">The garden item creation request.</param>
    [HttpPost]
    [ProducesResponseType(typeof(GardenItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GardenItemDto>> Create(CreateGardenItemRequest request)
    {
        try
        {
            var item = await _gardenItemService.CreateGardenItemAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing garden item's properties (dimensions, type, sunlight, nickname).
    /// Does not affect the plants assigned to the item.
    /// </summary>
    /// <param name="id">The garden item ID.</param>
    /// <param name="request">The update request.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GardenItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GardenItemDto>> Update(int id, UpdateGardenItemRequest request)
    {
        var item = await _gardenItemService.UpdateGardenItemAsync(id, request);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Deletes a garden item and all of its plant assignments.
    /// </summary>
    /// <param name="id">The garden item ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        return await _gardenItemService.DeleteGardenItemAsync(id) ? NoContent() : NotFound();
    }

    /// <summary>
    /// Assigns a plant to a garden item. The plant's spacing requirement must fit within the container's
    /// remaining available area after accounting for all previously assigned plants.
    /// Returns 422 if there is insufficient space or a sunlight mismatch.
    /// </summary>
    /// <param name="id">The garden item ID.</param>
    /// <param name="plantId">The ID of the plant to assign.</param>
    [HttpPut("{id:int}/plant/{plantId:int}")]
    [ProducesResponseType(typeof(GardenItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GardenItemDto>> AssignPlant(int id, int plantId)
    {
        try
        {
            return Ok(await _gardenItemService.AssignPlantAsync(id, plantId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }

    /// <summary>
    /// Removes a specific plant assignment from a garden item, freeing up its spacing area.
    /// The available area on the container is updated automatically.
    /// </summary>
    /// <param name="id">The garden item ID.</param>
    /// <param name="plantId">The ID of the plant to remove.</param>
    [HttpDelete("{id:int}/plant/{plantId:int}")]
    [ProducesResponseType(typeof(GardenItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GardenItemDto>> RemovePlant(int id, int plantId)
    {
        try
        {
            return Ok(await _gardenItemService.RemovePlantAsync(id, plantId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
