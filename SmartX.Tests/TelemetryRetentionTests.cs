using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Tests;

public class TelemetryRetentionTests
{
    private static TelemetryReading CreateReading(
        string deviceId,
        DateTime timestamp)
    {
        return new TelemetryReading
        {
            DeviceId = deviceId,
            Timestamp = timestamp,
            Category = SensorCategory.Environmental,
            Value = 25.0,
            Unit = "C"
        };
    }

    [Fact]
    public void History_KeepsOnlyLatest10000Readings()
    {
        var service = new ChronologicalTelemetryService();
        var start = DateTime.UtcNow;

        for (int i = 0; i < 10050; i++)
        {
            service.AddReading(CreateReading(
                $"DEVICE-{i:00000}",
                start.AddSeconds(i)));
        }

        var oldestRemoved = service.GetForDevice("DEVICE-00000");
        var latestPreserved = service.GetForDevice("DEVICE-10049");

        Assert.Empty(oldestRemoved);
        Assert.Single(latestPreserved);
        Assert.Equal(1000, service.GetRecent(1000).Count);
    }

    [Fact]
    public void History_HandlesDuplicateTimestampsAtCapacity()
    {
        var service = new ChronologicalTelemetryService();
        var timestamp = DateTime.UtcNow;

        for (int i = 0; i < 10001; i++)
        {
            service.AddReading(CreateReading(
                $"DEVICE-{i:00000}",
                timestamp));
        }

        Assert.Empty(service.GetForDevice("DEVICE-00000"));
        Assert.Single(service.GetForDevice("DEVICE-10000"));
        Assert.Equal(1000, service.GetRecent(1000).Count);
    }
}
