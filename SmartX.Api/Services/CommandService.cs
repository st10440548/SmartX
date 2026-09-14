using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class CommandService
{
    private readonly List<DeviceCommand> _commands = new();
    private readonly object _lock = new();

    public DeviceCommand CreateCommand(
        string deviceId,
        string commandType,
        string payload)
    {
        var command = new DeviceCommand
        {
            DeviceId = deviceId,
            CommandType = commandType,
            Payload = payload,
            Status = "Executed",
            ExecutedAt = DateTime.UtcNow
        };

        lock (_lock)
        {
            _commands.Add(command);
        }

        return command;
    }

    public List<DeviceCommand> GetAll()
    {
        lock (_lock)
        {
            return _commands
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }
    }

    public List<DeviceCommand> GetForDevice(
        string deviceId)
    {
        lock (_lock)
        {
            return _commands
                .Where(x =>
                    x.DeviceId.Equals(
                        deviceId,
                        StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }
    }
}
