using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class DeviceStateService
{
    private readonly Dictionary<string, DeviceState> _states =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly object _lock = new();

    public void UpdateState(TelemetryReading reading)
    {
        lock (_lock)
        {
            if (_states.TryGetValue(reading.DeviceId, out var existing)
                && reading.Timestamp < existing.LastUpdated)
            {
                return;
            }

            _states[reading.DeviceId] = new DeviceState
            {
                DeviceId = reading.DeviceId,
                Category = reading.Category,
                LatestValue = reading.Value,
                Unit = reading.Unit,
                LastUpdated = reading.Timestamp,
                IsAnomaly = reading.IsAnomaly,
                Severity = reading.Severity
            };
        }
    }

    public DeviceState? GetState(string deviceId)
    {
        lock (_lock)
        {
            return _states.TryGetValue(deviceId, out var state)
                ? state
                : null;
        }
    }

    public List<DeviceState> GetAllStates()
    {
        lock (_lock)
        {
            return _states.Values
                .OrderBy(x => x.DeviceId)
                .ToList();
        }
    }

    public int GetDeviceCount()
    {
        lock (_lock)
        {
            return _states.Count;
        }
    }
}
