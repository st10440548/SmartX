using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class ActiveAlertService
{
    private readonly HashSet<string> _activeAlerts =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, Guid> _activeAlertIds =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly object _lock = new();

    private long _duplicatesPrevented;

    private static string GetAlertKey(Alert alert)
    {
        return $"{alert.DeviceId}:{alert.Severity}";
    }

    public bool TryActivate(Alert alert)
    {
        lock (_lock)
        {
            var key = GetAlertKey(alert);

            if (_activeAlerts.Add(key))
            {
                _activeAlertIds[key] = alert.Id;
                return true;
            }

            _duplicatesPrevented++;
            return false;
        }
    }


    public bool IsActive(Alert alert)
    {
        lock (_lock)
        {
            return _activeAlertIds.TryGetValue(
                GetAlertKey(alert), out var activeId)
                && activeId == alert.Id;
        }
    }

    public bool Resolve(Alert alert)
    {
        lock (_lock)
        {
            var key = GetAlertKey(alert);

            if (!_activeAlertIds.TryGetValue(key, out var activeId)
                || activeId != alert.Id)
            {
                return false;
            }

            _activeAlertIds.Remove(key);
            return _activeAlerts.Remove(key);
        }
    }

    public object GetStatistics()
    {
        lock (_lock)
        {
            return new
            {
                ActiveCount = _activeAlerts.Count,
                DuplicatesPrevented = _duplicatesPrevented,
                ActiveKeys = _activeAlerts.OrderBy(x => x).ToList()
            };
        }
    }
}
