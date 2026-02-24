using AssetApi.Data;
using AssetApi.Entities;
using AssetApi.Messages;
using AssetApi.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AssetApi.Services;

public class AssetService : IAssetService
{
  private readonly AssetDbContext _context;
  private readonly IDistributedCache _cache;
  private readonly RabbitMqPublisher _publisher;

  private const string AllAssetsCacheKey = "assets:all";

  public AssetService(AssetDbContext context, IDistributedCache cache, RabbitMqPublisher publisher)
  {
    _context = context;
    _cache = cache;
    _publisher = publisher;
  }

  public async Task<IEnumerable<Asset>> GetAllAsync()
  {
    var cached = await _cache.GetStringAsync(AllAssetsCacheKey);
    if (cached is not null)
      return JsonSerializer.Deserialize<IEnumerable<Asset>>(cached)!;

    var assets = await _context.Assets.ToListAsync();
    await _cache.SetStringAsync(AllAssetsCacheKey,
        JsonSerializer.Serialize(assets),
        new DistributedCacheEntryOptions
        {
          AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
    return assets;
  }

  public async Task<Asset?> GetByIdAsync(int id)
      => await _context.Assets.FindAsync(id);

  public async Task<Asset> CreateAsync(Asset asset)
  {
    asset.CreatedAt = DateTime.UtcNow;
    _context.Assets.Add(asset);
    await _context.SaveChangesAsync();
    await _cache.RemoveAsync(AllAssetsCacheKey);

    _publisher.Publish("asset.created", new AssetCreatedMessage
    {
      AssetId = asset.Id,
      Name = asset.Name,
      CreatedAt = asset.CreatedAt
    });


    return asset;
  }

  public async Task<Asset?> UpdateAsync(int id, Asset asset)
  {
    var existing = await _context.Assets.FindAsync(id);
    if (existing is null) return null;
    existing.Name = asset.Name;
    existing.Description = asset.Description;
    await _context.SaveChangesAsync();
    await _cache.RemoveAsync(AllAssetsCacheKey);
    return existing;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var existing = await _context.Assets.FindAsync(id);
    if (existing is null) return false;
    _context.Assets.Remove(existing);
    await _context.SaveChangesAsync();
    await _cache.RemoveAsync(AllAssetsCacheKey);
    return true;
  }
}
