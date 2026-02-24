using AssetApi.Entities;
using AssetApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
  private readonly IAssetService _service;

  public AssetsController(IAssetService service)
  {
    _service = service;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Asset>>> GetAll()
      => Ok(await _service.GetAllAsync());

  [HttpGet("{id}")]
  public async Task<ActionResult<Asset>> GetById(int id)
  {
    var asset = await _service.GetByIdAsync(id);
    return asset is null ? NotFound() : Ok(asset);
  }

  [HttpPost]
  public async Task<ActionResult<Asset>> Create(CreateAssetRequest request)
  {
    var created = await _service.CreateAsync(new Asset
    {
      Name = request.Name,
      Description = request.Description
    });
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Asset>> Update(int id, Asset asset)
  {
    var updated = await _service.UpdateAsync(id, asset);
    return updated is null ? NotFound() : Ok(updated);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var deleted = await _service.DeleteAsync(id);
    return deleted ? NoContent() : NotFound();
  }
}
