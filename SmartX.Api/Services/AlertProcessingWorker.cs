using Microsoft.Extensions.Hosting;

namespace SmartX.Api.Services;

public class AlertProcessingWorker : BackgroundService
{
    private readonly PriorityAlertService _alerts;
    private readonly ILogger<AlertProcessingWorker> _logger;

    public AlertProcessingWorker(
        PriorityAlertService alerts,
        ILogger<AlertProcessingWorker> logger)
    {
        _alerts = alerts;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(1));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Drain pending alerts in priority order.
                while (!stoppingToken.IsCancellationRequested)
                {
                    var alert = _alerts.ProcessNext();

                    if (alert is null)
                        break;

                    _logger.LogDebug(
                        "Processed {Severity} alert for {DeviceId}",
                        alert.Severity,
                        alert.DeviceId);
                }
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Normal shutdown.
        }
    }
}
