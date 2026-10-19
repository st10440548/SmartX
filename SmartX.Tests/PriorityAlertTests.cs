using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Tests;

public class PriorityAlertTests
{
    private static Alert CreateAlert(
        string deviceId,
        AnomalySeverity severity)
    {
        return new Alert
        {
            DeviceId = deviceId,
            Severity = severity,
            Message = "Automated test alert",
            Timestamp = DateTime.UtcNow
        };
    }

    [Fact]
    public void CriticalAlerts_AreProcessedBeforeWarnings()
    {
        var active = new ActiveAlertService();
        var queue = new PriorityAlertService(active);

        var warning = CreateAlert(
            "DEVICE-001", AnomalySeverity.Warning);

        var critical = CreateAlert(
            "DEVICE-002", AnomalySeverity.Critical);

        Assert.True(active.TryActivate(warning));
        Assert.True(active.TryActivate(critical));

        queue.Enqueue(warning);
        queue.Enqueue(critical);

        var first = queue.ProcessNext();
        var second = queue.ProcessNext();

        Assert.Equal(critical.Id, first?.Id);
        Assert.Equal(warning.Id, second?.Id);
    }

    [Fact]
    public void EmptyQueue_ReturnsNull()
    {
        var active = new ActiveAlertService();
        var queue = new PriorityAlertService(active);

        Assert.Null(queue.ProcessNext());
    }

    [Fact]
    public void ResolvedAlerts_AreDiscarded()
    {
        var active = new ActiveAlertService();
        var queue = new PriorityAlertService(active);

        var alert = CreateAlert(
            "DEVICE-003", AnomalySeverity.Warning);

        Assert.True(active.TryActivate(alert));
        queue.Enqueue(alert);

        Assert.True(active.Resolve(alert));

        Assert.Null(queue.ProcessNext());
        Assert.False(alert.IsProcessed);
    }

    [Fact]
    public void ProcessedAlerts_AppearInHistory()
    {
        var active = new ActiveAlertService();
        var queue = new PriorityAlertService(active);

        var alert = CreateAlert(
            "DEVICE-004", AnomalySeverity.Critical);

        Assert.True(active.TryActivate(alert));
        queue.Enqueue(alert);

        var processed = queue.ProcessNext();
        var history = queue.GetProcessedAlerts();

        Assert.NotNull(processed);
        Assert.True(processed.IsProcessed);
        Assert.Single(history);
        Assert.Equal(alert.Id, history[0].Id);
    }
}
