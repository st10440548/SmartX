using SmartX.Api.Services;

namespace SmartX.Tests;

public class CommandUndoTests
{
    [Fact]
    public void OpenValve_ChangesActuatorState()
    {
        var actuators = new ActuatorStateService();
        var commands = new CommandService(actuators);

        commands.CreateCommand("VALVE-001", "OpenValve", "");

        Assert.True(actuators.GetState("VALVE-001"));
    }

    [Fact]
    public void Undo_RestoresPreviousActuatorState()
    {
        var actuators = new ActuatorStateService();
        var commands = new CommandService(actuators);

        commands.CreateCommand("VALVE-001", "OpenValve", "");

        var undo = commands.UndoLastCommand();

        Assert.NotNull(undo);
        Assert.False(actuators.GetState("VALVE-001"));
        Assert.True(undo.IsUndo);
    }

    [Fact]
    public void Undo_UsesLastInFirstOutOrder()
    {
        var actuators = new ActuatorStateService();
        var commands = new CommandService(actuators);

        var first = commands.CreateCommand(
            "VALVE-001", "OpenValve", "");

        var second = commands.CreateCommand(
            "VALVE-002", "OpenValve", "");

        var firstUndo = commands.UndoLastCommand();

        Assert.Equal(second.Id, firstUndo?.OriginalCommandId);
        Assert.False(actuators.GetState("VALVE-002"));
        Assert.True(actuators.GetState("VALVE-001"));

        var secondUndo = commands.UndoLastCommand();

        Assert.Equal(first.Id, secondUndo?.OriginalCommandId);
        Assert.False(actuators.GetState("VALVE-001"));
    }

    [Fact]
    public void Undo_ReturnsNullWhenStackIsEmpty()
    {
        var actuators = new ActuatorStateService();
        var commands = new CommandService(actuators);

        Assert.Null(commands.UndoLastCommand());
    }
}
