using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Tests;

public class TelemetryQueueTests
{
    private static TelemetryReading CreateReading(string deviceId)
    {
        return new TelemetryReading
        {
            DeviceId = deviceId,
            Timestamp = DateTime.UtcNow,
            Location = "Test Lab",
            Value = 25.5,
            Unit = "C"
        };
    }

    [Fact]
    public void Queue_ProcessesReadingsInFIFOOrder()
    {
        var queue = new TelemetryQueueService();

        queue.Enqueue(CreateReading("DEVICE-001"));
        queue.Enqueue(CreateReading("DEVICE-002"));
        queue.Enqueue(CreateReading("DEVICE-003"));

        Assert.True(queue.TryDequeue(out var first));
        Assert.True(queue.TryDequeue(out var second));
        Assert.True(queue.TryDequeue(out var third));

        Assert.Equal("DEVICE-001", first?.DeviceId);
        Assert.Equal("DEVICE-002", second?.DeviceId);
        Assert.Equal("DEVICE-003", third?.DeviceId);
    }

    [Fact]
    public void Queue_ReturnsFalseWhenEmpty()
    {
        var queue = new TelemetryQueueService();

        var result = queue.TryDequeue(out var reading);

        Assert.False(result);
        Assert.Null(reading);
    }

    [Fact]
    public void Queue_TracksStatisticsCorrectly()
    {
        var queue = new TelemetryQueueService();

        queue.Enqueue(CreateReading("DEVICE-001"));
        queue.Enqueue(CreateReading("DEVICE-002"));
        queue.Enqueue(CreateReading("DEVICE-003"));

        Assert.True(queue.TryDequeue(out _));

        var stats = queue.GetStatistics();

        Assert.Equal(3L, stats.TotalEnqueued);
        Assert.Equal(1L, stats.TotalProcessed);
        Assert.Equal(2, stats.Pending);
        Assert.Equal(
            stats.TotalEnqueued,
            stats.TotalProcessed + stats.Pending
        );
    }
}
