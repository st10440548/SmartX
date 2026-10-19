using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Tests;

public class DeviceStateTests
{
    private static TelemetryReading CreateReading(
        string deviceId,
        double value,
        DateTime timestamp)
    {
        return new TelemetryReading
        {
            DeviceId = deviceId,
            Timestamp = timestamp,
            Location = "Test Lab",
            Category = SensorCategory.Environmental,
            Value = value,
            Unit = "C",
            IsAnomaly = false,
            Severity = AnomalySeverity.None
        };
    }

    [Fact]
    public void Dictionary_AddsNewDevices()
    {
        var service = new DeviceStateService();
        var now = DateTime.UtcNow;

        service.UpdateState(CreateReading("DEVICE-001", 25, now));
        service.UpdateState(CreateReading("DEVICE-002", 30, now));

        Assert.Equal(2, service.GetDeviceCount());
        Assert.NotNull(service.GetState("DEVICE-001"));
        Assert.NotNull(service.GetState("DEVICE-002"));
    }

    [Fact]
    public void Dictionary_UpdatesExistingDeviceWithoutDuplicates()
    {
        var service = new DeviceStateService();
        var now = DateTime.UtcNow;

        service.UpdateState(CreateReading("DEVICE-001", 25, now));
        service.UpdateState(CreateReading("DEVICE-001", 35, now.AddSeconds(5)));

        Assert.Equal(1, service.GetDeviceCount());
        Assert.Equal(35.0, service.GetState("DEVICE-001")?.LatestValue);
    }

    [Fact]
    public void Dictionary_LookupIsCaseInsensitive()
    {
        var service = new DeviceStateService();

        service.UpdateState(
            CreateReading("DEVICE-001", 25, DateTime.UtcNow));

        var state = service.GetState("device-001");

        Assert.NotNull(state);
        Assert.Equal("DEVICE-001", state.DeviceId);
    }

    [Fact]
    public void Dictionary_RejectsOlderTelemetry()
    {
        var service = new DeviceStateService();
        var now = DateTime.UtcNow;

        service.UpdateState(CreateReading("DEVICE-001", 40, now));
        service.UpdateState(
            CreateReading("DEVICE-001", 10, now.AddMinutes(-5)));

        var state = service.GetState("DEVICE-001");

        Assert.NotNull(state);
        Assert.Equal(40.0, state.LatestValue);
        Assert.Equal(now, state.LastUpdated);
    }
}
