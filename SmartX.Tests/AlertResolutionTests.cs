using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Tests;

public class AlertResolutionTests
{
    private static Alert CreateAlert(string deviceId)
    {
        return new Alert
        {
            DeviceId = deviceId,
            Severity = AnomalySeverity.Critical,
            Message = "Alert resolution regression test"
        };
    }

    [Fact]
    public void OldAlert_CannotResolveNewerActiveAlert()
    {
        var service = new ActiveAlertService();

        var oldAlert = CreateAlert("DEVICE-001");
        var newAlert = CreateAlert("DEVICE-001");

        Assert.True(service.TryActivate(oldAlert));
        Assert.True(service.Resolve(oldAlert));

        Assert.True(service.TryActivate(newAlert));
        Assert.False(service.Resolve(oldAlert));

        Assert.True(service.IsActive(newAlert));
    }

    [Fact]
    public void RejectedDuplicate_CannotResolveActiveAlert()
    {
        var service = new ActiveAlertService();

        var original = CreateAlert("DEVICE-002");
        var duplicate = CreateAlert("device-002");

        Assert.True(service.TryActivate(original));
        Assert.False(service.TryActivate(duplicate));

        Assert.False(service.Resolve(duplicate));
        Assert.True(service.IsActive(original));
    }

    [Fact]
    public void ResolvingSameAlertTwice_ReturnsFalseSecondTime()
    {
        var service = new ActiveAlertService();

        var alert = CreateAlert("DEVICE-003");

        Assert.True(service.TryActivate(alert));
        Assert.True(service.Resolve(alert));

        Assert.False(service.Resolve(alert));
        Assert.False(service.IsActive(alert));
    }
}
