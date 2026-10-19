using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class PredictiveRecommendationService
{
    private readonly DeviceStateService _deviceStates;

    public PredictiveRecommendationService(
        DeviceStateService deviceStates)
    {
        _deviceStates = deviceStates;
    }

    public List<PredictiveRecommendation> GetRecommendations()
    {
        var recommendations = new List<PredictiveRecommendation>();

        foreach (var device in _deviceStates.GetAllStates())
        {
            if (!device.IsAnomaly)
                continue;

            string severity = device.Severity.ToString();

            if (device.Category == SensorCategory.Environmental)
            {
                recommendations.Add(new PredictiveRecommendation
                {
                    DeviceId = device.DeviceId,
                    Category = "Temperature",
                    Severity = severity,
                    Message = $"Abnormal temperature detected: {device.LatestValue} {device.Unit}",
                    RecommendedAction =
                        device.Severity == AnomalySeverity.Critical
                            ? "Inspect the sensor and surrounding equipment immediately."
                            : "Monitor temperature trends and inspect ventilation.",
                    Timestamp = device.LastUpdated
                });
            }
            else if (device.Category == SensorCategory.PowerConsumption)
            {
                recommendations.Add(new PredictiveRecommendation
                {
                    DeviceId = device.DeviceId,
                    Category = "Power Consumption",
                    Severity = severity,
                    Message = $"Abnormal power consumption detected: {device.LatestValue} {device.Unit}",
                    RecommendedAction =
                        device.Severity == AnomalySeverity.Critical
                            ? "Investigate possible electrical overload and isolate affected equipment if necessary."
                            : "Check connected devices for unusually high power usage.",
                    Timestamp = device.LastUpdated
                });
            }
        }

        return recommendations
            .OrderByDescending(r => r.Severity == "Critical")
            .ThenByDescending(r => r.Timestamp)
            .ToList();
    }
}
