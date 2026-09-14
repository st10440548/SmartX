namespace SmartX.Api.Models;

public class Sensor
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DeviceId { get; set; } = string.Empty;

    public string MacAddress { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string DeploymentLocation { get; set; } = string.Empty;

    public string Zone { get; set; } = string.Empty;

    public string NodeId { get; set; } = string.Empty;

    public SensorCategory Category { get; set; }

    public string DataType { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public bool IsOnline { get; set; }

    public DateTime LastSeen { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
