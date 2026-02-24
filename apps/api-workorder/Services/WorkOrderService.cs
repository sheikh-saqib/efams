using Microsoft.EntityFrameworkCore;
using WorkOrderApi.Data;
using WorkOrderApi.Entities;

namespace WorkOrderApi.Services;

public class WorkOrderService : IWorkOrderService
{
    private readonly WorkOrderDbContext _context;

    public WorkOrderService(WorkOrderDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WorkOrder>> GetAllAsync()
        => await _context.WorkOrders.ToListAsync();

    public async Task<WorkOrder?> GetByIdAsync(int id)
        => await _context.WorkOrders.FindAsync(id);

    public async Task<WorkOrder> CreateAsync(WorkOrder workOrder)
    {
        workOrder.CreatedAt = DateTime.UtcNow;
        workOrder.Status ??= "Open";
        _context.WorkOrders.Add(workOrder);
        await _context.SaveChangesAsync();
        return workOrder;
    }

    public async Task<WorkOrder?> UpdateAsync(int id, WorkOrder workOrder)
    {
        var existing = await _context.WorkOrders.FindAsync(id);
        if (existing is null) return null;
        existing.Title = workOrder.Title;
        existing.Description = workOrder.Description;
        existing.Status = workOrder.Status;
        existing.AssignedTo = workOrder.AssignedTo;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.WorkOrders.FindAsync(id);
        if (existing is null) return false;
        _context.WorkOrders.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}