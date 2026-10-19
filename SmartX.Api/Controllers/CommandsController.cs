using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommandsController : ControllerBase
{
    private readonly CommandService _commandService;
    private readonly SensorService _sensorService;

    public CommandsController(
        CommandService commandService,
        SensorService sensorService)
    {
        _commandService = commandService;
        _sensorService = sensorService;
    }

    [HttpPost]
    public IActionResult SendCommand(
        [FromBody] CreateCommandRequest request)
    {
        var sensor =
            _sensorService.GetByDeviceId(request.DeviceId);

        if (sensor is null)
        {
            return NotFound(new
            {
                message = "Sensor not found."
            });
        }

        if (string.IsNullOrWhiteSpace(request.CommandType))
        {
            return BadRequest(new
            {
                message = "Command type is required."
            });
        }

        var command =
            _commandService.CreateCommand(
                request.DeviceId,
                request.CommandType,
                request.Payload);

        return Ok(command);
    }


    [HttpPost("undo")]
    public IActionResult UndoLastCommand()
    {
        var command = _commandService.UndoLastCommand();

        if (command is null)
        {
            return Conflict(new
            {
                message = "No reversible commands available."
            });
        }

        return Ok(command);
    }

    [HttpGet]
    public IActionResult GetCommands()
    {
        return Ok(_commandService.GetAll());
    }

    [HttpGet("{deviceId}")]
    public IActionResult GetDeviceCommands(
        string deviceId)
    {
        var sensor =
            _sensorService.GetByDeviceId(deviceId);

        if (sensor is null)
        {
            return NotFound(new
            {
                message = "Sensor not found."
            });
        }

        return Ok(
            _commandService.GetForDevice(deviceId));
    }
}

public class CreateCommandRequest
{
    public string DeviceId { get; set; } = string.Empty;

    public string CommandType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;
}
