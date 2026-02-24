using FacilityApi.Entities;

namespace FacilityApi.Services;

public interface IFacilityService
{
  Task<IEnumerable<Facility>> GetAllAsync();
  Task<Facility?> GetByIdAsync(int id);
  Task<Facility> CreateAsync(Facility facility);
  Task<Facility?> UpdateAsync(int id, Facility facility);
  Task<bool> DeleteAsync(int id);
}
