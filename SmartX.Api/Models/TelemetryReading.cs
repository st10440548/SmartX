namespace SmartX.Api.Models;

public class TelemetryReading
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DeviceId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public string Location { get; set; } = string.Empty;

    public SensorCategory Category { get; set; }

    public string DataType { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public object? Value { get; set; }

    public bool IsAnomaly { get; set; }

    public AnomalySeverity Severity { get; set; } = AnomalySeverity.None;
}
