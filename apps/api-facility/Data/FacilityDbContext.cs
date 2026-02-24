using Microsoft.EntityFrameworkCore;
using FacilityApi.Entities;

namespace FacilityApi.Data;

public class FacilityDbContext : DbContext
{
    public FacilityDbContext(DbContextOptions<FacilityDbContext> options) : base(options) { }

    public DbSet<Facility> Facilities => Set<Facility>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasIndex(e => e.Name).HasDatabaseName("IX_Facilities_Name");
        });
    }
}