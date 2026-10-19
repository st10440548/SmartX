namespace SmartX.Api.Models;

public class TelemetryRecord
{
    public string DeviceId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public SensorCategory Category { get; set; }

    public object? Value { get; set; }

    public string Unit { get; set; } = string.Empty;

    public bool IsAnomaly { get; set; }

    public AnomalySeverity Severity { get; set; }
}
