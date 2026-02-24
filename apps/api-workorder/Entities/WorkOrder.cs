namespace WorkOrderApi.Entities;

public class WorkOrder
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
}