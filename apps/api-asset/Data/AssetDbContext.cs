using Microsoft.EntityFrameworkCore;
using AssetApi.Entities;

namespace AssetApi.Data;

public class AssetDbContext : DbContext
{
    public AssetDbContext(DbContextOptions<AssetDbContext> options) : base(options) { }

    public DbSet<Asset> Assets => Set<Asset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasIndex(e => e.Name).HasDatabaseName("IX_Assets_Name");
        });
    }
}