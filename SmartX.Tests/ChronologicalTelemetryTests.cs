using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Tests;

public class ChronologicalTelemetryTests
{
    private static TelemetryReading CreateReading(
        string deviceId,
        DateTime timestamp,
        double value)
    {
        return new TelemetryReading
        {
            DeviceId = deviceId,
            Timestamp = timestamp,
            Category = SensorCategory.Environmental,
            Value = value,
            Unit = "C"
        };
    }

    [Fact]
    public void RecentReadings_AreNewestFirst()
    {
        var service = new ChronologicalTelemetryService();
        var now = DateTime.UtcNow;

        service.AddReading(
            CreateReading("DEVICE-001", now.AddMinutes(-10), 20));

        service.AddReading(
            CreateReading("DEVICE-002", now, 30));

        service.AddReading(
            CreateReading("DEVICE-003", now.AddMinutes(-5), 25));

        var readings = service.GetRecent();

        Assert.Equal(3, readings.Count);
        Assert.Equal("DEVICE-002", readings[0].DeviceId);
        Assert.Equal("DEVICE-003", readings[1].DeviceId);
        Assert.Equal("DEVICE-001", readings[2].DeviceId);
    }

    [Fact]
    public void Readings_CanBeFilteredByDevice()
    {
        var service = new ChronologicalTelemetryService();
        var now = DateTime.UtcNow;

        service.AddReading(CreateReading("DEVICE-001", now, 20));
        service.AddReading(CreateReading("DEVICE-002", now, 30));
        service.AddReading(
            CreateReading("DEVICE-001", now.AddSeconds(1), 25));

        var readings = service.GetForDevice("device-001");

        Assert.Equal(2, readings.Count);
        Assert.All(
            readings,
            reading => Assert.Equal("DEVICE-001", reading.DeviceId));
    }

    [Fact]
    public void IdenticalTimestamps_PreserveAllReadings()
    {
        var service = new ChronologicalTelemetryService();
        var timestamp = DateTime.UtcNow;

        service.AddReading(
            CreateReading("DEVICE-001", timestamp, 20));

        service.AddReading(
            CreateReading("DEVICE-002", timestamp, 30));

        var readings = service.GetRecent();

        Assert.Equal(2, readings.Count);
        Assert.Contains(readings, r => r.DeviceId == "DEVICE-001");
        Assert.Contains(readings, r => r.DeviceId == "DEVICE-002");
    }

    [Fact]
    public void GetRecent_RespectsRequestedLimit()
    {
        var service = new ChronologicalTelemetryService();
        var now = DateTime.UtcNow;

        for (int i = 0; i < 10; i++)
        {
            service.AddReading(
                CreateReading(
                    $"DEVICE-{i:000}",
                    now.AddSeconds(i),
                    i));
        }

        var readings = service.GetRecent(3);

        Assert.Equal(3, readings.Count);
        Assert.Equal("DEVICE-009", readings[0].DeviceId);
        Assert.Equal("DEVICE-008", readings[1].DeviceId);
        Assert.Equal("DEVICE-007", readings[2].DeviceId);
    }
}
