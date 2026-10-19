using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class CommandService
{
    private readonly List<DeviceCommand> _commands = new();
    private readonly Stack<CommandRecord> _undoStack = new();
    private readonly ActuatorStateService _actuatorStates;
    private readonly object _lock = new();

    public CommandService(ActuatorStateService actuatorStates)
    {
        _actuatorStates = actuatorStates;
    }

    public DeviceCommand CreateCommand(
        string deviceId,
        string commandType,
        string payload)
    {
        lock (_lock)
        {
            var command = new DeviceCommand
            {
                DeviceId = deviceId,
                CommandType = commandType,
                Payload = payload ?? string.Empty,
                Status = "Executed",
                ExecutedAt = DateTime.UtcNow
            };

            if (commandType == "OpenValve" ||
                commandType == "CloseValve")
            {
                bool newState = commandType == "OpenValve";

                command.PreviousState =
                    _actuatorStates.SetState(deviceId, newState);

                command.NewState = newState;

                command.ResultMessage =
                    newState ? "Actuator opened." : "Actuator closed.";

                _undoStack.Push(new CommandRecord
                {
                    CommandId = command.Id,
                    DeviceId = command.DeviceId,
                    CommandType = command.CommandType,
                    PreviousState = command.PreviousState!.Value,
                    NewState = command.NewState!.Value,
                    Timestamp = command.CreatedAt
                });
            }
            else
            {
                command.ResultMessage =
                    "Command recorded in simulation.";
            }

            _commands.Add(command);
            return command;
        }
    }

    public DeviceCommand? UndoLastCommand()
    {
        lock (_lock)
        {
            if (_undoStack.Count == 0)
                return null;

            var original = _undoStack.Pop();

            bool restoredState = original.PreviousState;

            _actuatorStates.SetState(
                original.DeviceId, restoredState);

            var originalCommand = _commands.FirstOrDefault(
                command => command.Id == original.CommandId);

            if (originalCommand is not null)
                originalCommand.Status = "Undone";

            var undoCommand = new DeviceCommand
            {
                DeviceId = original.DeviceId,
                CommandType = "Undo",
                Payload = original.CommandType,
                Status = "Executed",
                ExecutedAt = DateTime.UtcNow,
                IsUndo = true,
                OriginalCommandId = original.CommandId,
                PreviousState = original.NewState,
                NewState = restoredState,
                ResultMessage = "Previous actuator state restored."
            };

            _commands.Add(undoCommand);
            return undoCommand;
        }
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

    public List<DeviceCommand> GetForDevice(string deviceId)
    {
        lock (_lock)
        {
            return _commands
                .Where(x => x.DeviceId.Equals(
                    deviceId,
                    StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }
    }
}
