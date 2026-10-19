using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/chronological-telemetry")]
public class ChronologicalTelemetryController : ControllerBase
{
    private readonly ChronologicalTelemetryService _history;

    public ChronologicalTelemetryController(
        ChronologicalTelemetryService history)
    {
        _history = history;
    }

    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        return Ok(_history.GetStatistics());
    }

    [HttpGet("recent")]
    public IActionResult GetRecent([FromQuery] int count = 100)
    {
        return Ok(_history.GetRecent(count));
    }

    [HttpGet("device/{deviceId}")]
    public IActionResult GetForDevice(string deviceId)
    {
        return Ok(_history.GetForDevice(deviceId));
    }
}
