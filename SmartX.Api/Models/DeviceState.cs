namespace SmartX.Api.Models;

public class DeviceState
{
    public string DeviceId { get; set; } = string.Empty;

    public SensorCategory Category { get; set; }

    public object? LatestValue { get; set; }

    public string Unit { get; set; } = string.Empty;

    public DateTime LastUpdated { get; set; }

    public bool IsAnomaly { get; set; }

    public AnomalySeverity Severity { get; set; }
}
