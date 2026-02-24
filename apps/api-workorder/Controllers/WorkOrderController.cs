using Microsoft.AspNetCore.Mvc;
using WorkOrderApi.Entities;
using WorkOrderApi.Services;

namespace WorkOrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    private readonly IWorkOrderService _service;

    public WorkOrdersController(IWorkOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkOrder>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkOrder>> GetById(int id)
    {
        var workOrder = await _service.GetByIdAsync(id);
        return workOrder is null ? NotFound() : Ok(workOrder);
    }

    [HttpPost]
    public async Task<ActionResult<WorkOrder>> Create(WorkOrder workOrder)
    {
        var created = await _service.CreateAsync(workOrder);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<WorkOrder>> Update(int id, WorkOrder workOrder)
    {
        var updated = await _service.UpdateAsync(id, workOrder);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}