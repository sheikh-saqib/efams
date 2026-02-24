namespace FacilityApi.Entities;

public class Facility
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}