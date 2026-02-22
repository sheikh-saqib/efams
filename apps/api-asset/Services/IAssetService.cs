using AssetApi.Entities;

namespace AssetApi.Services;

public interface IAssetService
{
  Task<IEnumerable<Asset>> GetAllAsync();
  Task<Asset?> GetByIdAsync(int id);
}
