namespace SmartX.Api.Models;

public class DeviceCommand
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DeviceId { get; set; } = string.Empty;

    public string CommandType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExecutedAt { get; set; }

    // Part 2: previous and resulting actuator states
    public bool? PreviousState { get; set; }

    public bool? NewState { get; set; }

    // Identifies commands created to reverse an earlier command
    public bool IsUndo { get; set; }

    public Guid? OriginalCommandId { get; set; }

    // Records execution failures or additional information
    public string ResultMessage { get; set; } = string.Empty;
}
