namespace SmartX.Api.Models;

public class CommandRecord
{
    public Guid CommandId { get; set; }

    public string DeviceId { get; set; } = string.Empty;

    public string CommandType { get; set; } = string.Empty;

    public bool PreviousState { get; set; }

    public bool NewState { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
