using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly PriorityAlertService _alerts;
    private readonly ActiveAlertService _activeAlerts;

    public AlertsController(
        PriorityAlertService alerts,
        ActiveAlertService activeAlerts)
    {
        _alerts = alerts;
        _activeAlerts = activeAlerts;
    }


    [HttpGet("active")]
    public IActionResult GetActiveAlerts()
    {
        return Ok(_activeAlerts.GetStatistics());
    }

    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        return Ok(_alerts.GetStatistics());
    }

    [HttpGet("processed")]
    public IActionResult GetProcessed()
    {
        return Ok(_alerts.GetProcessedAlerts());
    }

    [HttpPost("process-next")]
    public IActionResult ProcessNext()
    {
        var alert = _alerts.ProcessNext();

        if (alert is null)
            return NoContent();

        return Ok(alert);
    }
}
