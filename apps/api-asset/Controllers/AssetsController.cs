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
}
