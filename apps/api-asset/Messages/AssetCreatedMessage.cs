namespace AssetApi.Messages;

public class AssetCreatedMessage
{
    public int AssetId { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
}