namespace SmartX.Api.Models;

public class MeshNode
{
    public string NodeId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string NodeType { get; set; } = "Sensor";

    public string ParentNodeId { get; set; } = string.Empty;

    public List<string> ChildNodeIds { get; set; } = new();

    public int SignalStrength { get; set; }

    public int HopCount { get; set; }

    public bool IsOnline { get; set; }

    public string RouteStatus { get; set; } = "Healthy";
}
