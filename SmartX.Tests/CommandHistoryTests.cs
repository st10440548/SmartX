using SmartX.Api.Services;

namespace SmartX.Tests;

public class CommandHistoryTests
{
    [Fact]
    public void SimulatedCommand_DoesNotEnterUndoStack()
    {
        var actuators = new ActuatorStateService();
        var commands = new CommandService(actuators);

        var valveCommand = commands.CreateCommand(
            "VALVE-001", "OpenValve", "");

        var simulatedCommand = commands.CreateCommand(
            "VALVE-001", "CheckStatus", "");

        var undo = commands.UndoLastCommand();

        Assert.Equal(
            valveCommand.Id,
            undo?.OriginalCommandId);

        Assert.False(actuators.GetState("VALVE-001"));
        Assert.Equal("Executed", simulatedCommand.Status);
        Assert.Null(commands.UndoLastCommand());
    }

    [Fact]
    public void Undo_MarksOriginalCommandAsUndone()
    {
        var actuators = new ActuatorStateService();
        var commands = new CommandService(actuators);

        var original = commands.CreateCommand(
            "VALVE-002", "OpenValve", "");

        Assert.Equal("Executed", original.Status);

        commands.UndoLastCommand();

        Assert.Equal("Undone", original.Status);
    }

    [Fact]
    public void Undo_RecordsRestoredStateAndOriginalCommand()
    {
        var actuators = new ActuatorStateService();
        var commands = new CommandService(actuators);

        var original = commands.CreateCommand(
            "VALVE-003", "OpenValve", "");

        var undo = commands.UndoLastCommand();

        Assert.NotNull(undo);
        Assert.True(undo.IsUndo);
        Assert.Equal(original.Id, undo.OriginalCommandId);
        Assert.Equal("Undo", undo.CommandType);
        Assert.True(undo.PreviousState);
        Assert.False(undo.NewState);
        Assert.False(actuators.GetState("VALVE-003"));
    }
}
