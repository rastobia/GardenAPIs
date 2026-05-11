using GardenApi.Application.DTOs.Garden;
using GardenApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GardensController : ControllerBase
{
    private readonly IGardenService _gardenService;

    public GardensController(IGardenService gardenService)
    {
        _gardenService = gardenService;
    }

    /// <summary>
    /// Returns all gardens with a count of their garden items.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GardenDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GardenDto>>> GetAll()
    {
        return Ok(await _gardenService.GetAllGardensAsync());
    }

    /// <summary>
    /// Returns a single garden by its ID.
    /// </summary>
    /// <param name="id">The garden ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GardenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GardenDto>> GetById(int id)
    {
        var garden = await _gardenService.GetGardenByIdAsync(id);
        return garden is null ? NotFound() : Ok(garden);
    }

    /// <summary>
    /// Creates a new garden.
    /// </summary>
    /// <param name="request">The garden creation request.</param>
    [HttpPost]
    [ProducesResponseType(typeof(GardenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GardenDto>> Create(CreateGardenRequest request)
    {
        var garden = await _gardenService.CreateGardenAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = garden.Id }, garden);
    }

    /// <summary>
    /// Updates an existing garden's name or description.
    /// </summary>
    /// <param name="id">The garden ID.</param>
    /// <param name="request">The update request.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GardenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GardenDto>> Update(int id, UpdateGardenRequest request)
    {
        var garden = await _gardenService.UpdateGardenAsync(id, request);
        return garden is null ? NotFound() : Ok(garden);
    }

    /// <summary>
    /// Deletes a garden and all of its garden items (cascade delete).
    /// </summary>
    /// <param name="id">The garden ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        return await _gardenService.DeleteGardenAsync(id) ? NoContent() : NotFound();
    }
}
