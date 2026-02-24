using FacilityApi.Entities;
using FacilityApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FacilityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacilitiesController : ControllerBase
{
  private readonly IFacilityService _service;

  public FacilitiesController(IFacilityService service)
  {
    _service = service;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Facility>>> GetAll()
      => Ok(await _service.GetAllAsync());

  [HttpGet("{id}")]
  public async Task<ActionResult<Facility>> GetById(int id)
  {
    var facility = await _service.GetByIdAsync(id);
    return facility is null ? NotFound() : Ok(facility);
  }

  [HttpPost]
  public async Task<ActionResult<Facility>> Create(Facility facility)
  {
    var created = await _service.CreateAsync(facility);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Facility>> Update(int id, Facility facility)
  {
    var updated = await _service.UpdateAsync(id, facility);
    return updated is null ? NotFound() : Ok(updated);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var deleted = await _service.DeleteAsync(id);
    return deleted ? NoContent() : NotFound();
  }
}
