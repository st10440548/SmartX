using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/telemetry-queue")]
public class TelemetryQueueController : ControllerBase
{
    private readonly TelemetryQueueService _queue;

    public TelemetryQueueController(TelemetryQueueService queue)
    {
        _queue = queue;
    }

    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        return Ok(_queue.GetStatistics());
    }
}
