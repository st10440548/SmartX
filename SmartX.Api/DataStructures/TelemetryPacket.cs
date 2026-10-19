namespace SmartX.Api.DataStructures;

public class TelemetryPacket
{
    public string DeviceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Location { get; set; } = string.Empty;
    public string SensorCategory { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public object? Value { get; set; }
}

public class TelemetryPacket<T>
{
    public string DeviceId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public string Location { get; set; } = string.Empty;

    public string SensorCategory { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public T Value { get; set; } = default!;
}
