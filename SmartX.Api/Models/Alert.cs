namespace SmartX.Api.Models;

public class Alert
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DeviceId { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public AnomalySeverity Severity { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public bool IsProcessed { get; set; } = false;
}
