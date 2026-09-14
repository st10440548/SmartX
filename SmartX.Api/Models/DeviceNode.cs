namespace SmartX.Api.Models;

public class DeviceNode
{
    public string Name { get; set; } = string.Empty;

    public string NodeType { get; set; } = string.Empty;

    public bool IsConfigured { get; set; }

    public List<DeviceNode> Children { get; set; } = new();
}
