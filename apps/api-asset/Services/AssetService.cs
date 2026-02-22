using AssetApi.Data;
using AssetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetApi.Services;

public class AssetService : IAssetService
{
  private readonly AssetDbContext _context;

  public AssetService(AssetDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Asset>> GetAllAsync()
      => await _context.Assets.ToListAsync();

  public async Task<Asset?> GetByIdAsync(int id)
      => await _context.Assets.FindAsync(id);
}
