using System.Collections.Concurrent;
using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Tests;

public class TelemetryConcurrencyTests
{
    private static TelemetryReading CreateReading(int id)
    {
        return new TelemetryReading
        {
            DeviceId = $"DEVICE-{id}",
            Timestamp = DateTime.UtcNow,
            Location = "Concurrency Test Lab",
            Value = 25.5,
            Unit = "C"
        };
    }

    [Fact]
    public void ConcurrentProducers_DoNotLoseReadings()
    {
        var queue = new TelemetryQueueService();

        Parallel.For(0, 2000, i =>
        {
            queue.Enqueue(CreateReading(i));
        });

        var stats = queue.GetStatistics();

        Assert.Equal(2000L, stats.TotalEnqueued);
        Assert.Equal(2000, stats.Pending);

        var deviceIds = new HashSet<string>();

        while (queue.TryDequeue(out var reading))
        {
            Assert.NotNull(reading);
            Assert.True(deviceIds.Add(reading.DeviceId));
        }

        Assert.Equal(2000, deviceIds.Count);

        stats = queue.GetStatistics();

        Assert.Equal(2000L, stats.TotalProcessed);
        Assert.Equal(0, stats.Pending);
    }

    [Fact]
    public void ConcurrentConsumers_DoNotProcessDuplicates()
    {
        var queue = new TelemetryQueueService();

        for (int i = 0; i < 2000; i++)
        {
            queue.Enqueue(CreateReading(i));
        }

        var processed = new ConcurrentBag<string>();

        Parallel.For(0, 8, _ =>
        {
            while (queue.TryDequeue(out var reading))
            {
                Assert.NotNull(reading);
                processed.Add(reading.DeviceId);
            }
        });

        Assert.Equal(2000, processed.Count);
        Assert.Equal(
            2000,
            processed.Distinct().Count());

        var stats = queue.GetStatistics();

        Assert.Equal(2000L, stats.TotalEnqueued);
        Assert.Equal(2000L, stats.TotalProcessed);
        Assert.Equal(0, stats.Pending);
    }
}
