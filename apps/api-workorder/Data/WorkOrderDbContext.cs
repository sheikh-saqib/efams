using Microsoft.EntityFrameworkCore;
using WorkOrderApi.Entities;

namespace WorkOrderApi.Data;

public class WorkOrderDbContext : DbContext
{
    public WorkOrderDbContext(DbContextOptions<WorkOrderDbContext> options) : base(options) { }

    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.HasIndex(e => e.AssetId).HasDatabaseName("IX_WorkOrders_AssetId");
        });
    }
}