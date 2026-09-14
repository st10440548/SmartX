using Microsoft.AspNetCore.Mvc;
using SmartX.Api.DataStructures;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryService _telemetryService;

    public TelemetryController(TelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    [HttpPost("temperature")]
    public IActionResult ReceiveTemperature(TelemetryPacket<float> packet)
    {
        _telemetryService.AddTemperature(packet);

        return Ok(new
        {
            message = "Temperature telemetry received",
            packet.DeviceId,
            packet.Value,
            packet.Timestamp
        });
    }

    [HttpPost("power")]
    public IActionResult ReceivePower(TelemetryPacket<int> packet)
    {
        _telemetryService.AddPower(packet);

        return Ok(new
        {
            message = "Power telemetry received",
            packet.DeviceId,
            packet.Value,
            packet.Timestamp
        });
    }

    [HttpPost("actuator")]
    public IActionResult ReceiveActuator(TelemetryPacket<bool> packet)
    {
        _telemetryService.AddActuator(packet);

        return Ok(new
        {
            message = "Actuator telemetry received",
            packet.DeviceId,
            packet.Value,
            packet.Timestamp
        });
    }

    [HttpGet]
    public IActionResult GetTelemetry()
    {
        return Ok(_telemetryService.GetAll());
    }

    [HttpGet("{deviceId}")]
    public IActionResult GetDeviceTelemetry(string deviceId)
    {
        return Ok(_telemetryService.GetForDevice(deviceId));
    }

    [HttpGet("anomalies")]
    public IActionResult GetAnomalies()
    {
        return Ok(_telemetryService.GetAnomalies());
    }
}
