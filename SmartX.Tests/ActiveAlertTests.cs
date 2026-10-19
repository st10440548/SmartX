using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Tests;

public class ActiveAlertTests
{
    private static Alert CreateAlert(
        string deviceId,
        AnomalySeverity severity)
    {
        return new Alert
        {
            DeviceId = deviceId,
            Severity = severity,
            Message = "Automated test alert"
        };
    }

    [Fact]
    public void NewAlert_IsActivated()
    {
        var service = new ActiveAlertService();
        var alert = CreateAlert(
            "DEVICE-001", AnomalySeverity.Warning);

        Assert.True(service.TryActivate(alert));
        Assert.True(service.IsActive(alert));
    }

    [Fact]
    public void DuplicateAlert_IsRejected()
    {
        var service = new ActiveAlertService();

        var first = CreateAlert(
            "DEVICE-001", AnomalySeverity.Warning);

        var duplicate = CreateAlert(
            "device-001", AnomalySeverity.Warning);

        Assert.True(service.TryActivate(first));
        Assert.False(service.TryActivate(duplicate));

        Assert.True(service.IsActive(first));
        Assert.False(service.IsActive(duplicate));
    }

    [Fact]
    public void ResolvedAlert_CanBeActivatedAgain()
    {
        var service = new ActiveAlertService();

        var first = CreateAlert(
            "DEVICE-002", AnomalySeverity.Critical);

        var second = CreateAlert(
            "DEVICE-002", AnomalySeverity.Critical);

        Assert.True(service.TryActivate(first));
        Assert.True(service.Resolve(first));

        Assert.True(service.TryActivate(second));
        Assert.True(service.IsActive(second));
    }

    [Fact]
    public void OldAlertId_IsNotConsideredActive()
    {
        var service = new ActiveAlertService();

        var oldAlert = CreateAlert(
            "DEVICE-003", AnomalySeverity.Warning);

        var newAlert = CreateAlert(
            "DEVICE-003", AnomalySeverity.Warning);

        Assert.True(service.TryActivate(oldAlert));
        Assert.True(service.Resolve(oldAlert));
        Assert.True(service.TryActivate(newAlert));

        Assert.False(service.IsActive(oldAlert));
        Assert.True(service.IsActive(newAlert));
    }
}
