using FacilityApi.Data;
using FacilityApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacilityApi.Services;

public class FacilityService : IFacilityService
{
  private readonly FacilityDbContext _context;

  public FacilityService(FacilityDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Facility>> GetAllAsync()
      => await _context.Facilities.ToListAsync();

  public async Task<Facility?> GetByIdAsync(int id)
      => await _context.Facilities.FindAsync(id);

  public async Task<Facility> CreateAsync(Facility facility)
  {
    facility.CreatedAt = DateTime.UtcNow;
    _context.Facilities.Add(facility);
    await _context.SaveChangesAsync();
    return facility;
  }

  public async Task<Facility?> UpdateAsync(int id, Facility facility)
  {
    var existing = await _context.Facilities.FindAsync(id);
    if (existing is null) return null;
    existing.Name = facility.Name;
    existing.Address = facility.Address;
    await _context.SaveChangesAsync();
    return existing;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var existing = await _context.Facilities.FindAsync(id);
    if (existing is null) return false;
    _context.Facilities.Remove(existing);
    await _context.SaveChangesAsync();
    return true;
  }
}
