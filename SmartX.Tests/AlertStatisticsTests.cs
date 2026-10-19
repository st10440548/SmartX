using System.Text.Json;
using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Tests;

public class AlertStatisticsTests
{
    [Fact]
    public void Statistics_RemainAccurateAfterHistoryLimit()
    {
        var active = new ActiveAlertService();
        var queue = new PriorityAlertService(active);

        for (int i = 0; i < 1100; i++)
        {
            var alert = new Alert
            {
                DeviceId = $"DEVICE-{i:0000}",
                Severity = i < 600
                    ? AnomalySeverity.Critical
                    : AnomalySeverity.Warning,
                Message = "Automated statistics test"
            };

            Assert.True(active.TryActivate(alert));
            queue.Enqueue(alert);
        }

        for (int i = 0; i < 1100; i++)
        {
            Assert.NotNull(queue.ProcessNext());
        }

        var history = queue.GetProcessedAlerts();

        Assert.Equal(1000, history.Count);

        var stats = JsonSerializer.SerializeToElement(
            queue.GetStatistics());

        Assert.Equal(
            1100,
            stats.GetProperty("TotalEnqueued").GetInt64());

        Assert.Equal(
            1100,
            stats.GetProperty("TotalProcessed").GetInt64());

        Assert.Equal(
            600,
            stats.GetProperty("CriticalProcessed").GetInt64());

        Assert.Equal(
            500,
            stats.GetProperty("WarningProcessed").GetInt64());

        Assert.Equal(
            0,
            stats.GetProperty("Pending").GetInt32());
    }

    [Fact]
    public void DiscardedAlerts_DoNotIncreaseProcessedCounters()
    {
        var active = new ActiveAlertService();
        var queue = new PriorityAlertService(active);

        var alert = new Alert
        {
            DeviceId = "DEVICE-001",
            Severity = AnomalySeverity.Critical
        };

        Assert.True(active.TryActivate(alert));
        queue.Enqueue(alert);
        Assert.True(active.Resolve(alert));

        Assert.Null(queue.ProcessNext());

        var stats = JsonSerializer.SerializeToElement(
            queue.GetStatistics());

        Assert.Equal(
            1,
            stats.GetProperty("TotalDiscarded").GetInt64());

        Assert.Equal(
            0,
            stats.GetProperty("TotalProcessed").GetInt64());

        Assert.Equal(
            0,
            stats.GetProperty("CriticalProcessed").GetInt64());
    }
}
