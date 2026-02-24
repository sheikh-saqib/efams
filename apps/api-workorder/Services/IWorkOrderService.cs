using WorkOrderApi.Entities;

namespace WorkOrderApi.Services;

public interface IWorkOrderService
{
    Task<IEnumerable<WorkOrder>> GetAllAsync();
    Task<WorkOrder?> GetByIdAsync(int id);
    Task<WorkOrder> CreateAsync(WorkOrder workOrder);
    Task<WorkOrder?> UpdateAsync(int id, WorkOrder workOrder);
    Task<bool> DeleteAsync(int id);
}