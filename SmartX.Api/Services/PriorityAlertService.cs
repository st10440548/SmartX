using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class PriorityAlertService
{
    private readonly PriorityQueue<Alert, int> _queue = new();
    private readonly List<Alert> _processedAlerts = new();
    private readonly object _lock = new();

    private readonly ActiveAlertService _activeAlerts;

    public PriorityAlertService(ActiveAlertService activeAlerts)
    {
        _activeAlerts = activeAlerts;
    }

    private long _totalEnqueued;
    private long _totalProcessed;
    private long _totalDiscarded;

    public void Enqueue(Alert alert)
    {
        lock (_lock)
        {
            int priority = alert.Severity switch
            {
                AnomalySeverity.Critical => 1,
                AnomalySeverity.Warning => 2,
                _ => 3
            };

            _queue.Enqueue(alert, priority);
            _totalEnqueued++;
        }
    }

    public Alert? ProcessNext()
    {
        lock (_lock)
        {
            while (_queue.Count > 0)
            {
                var alert = _queue.Dequeue();

                if (!_activeAlerts.IsActive(alert))
                {
                    _totalDiscarded++;
                    continue;
                }

                alert.IsProcessed = true;
                _processedAlerts.Add(alert);
                _totalProcessed++;

                if (_processedAlerts.Count > 1000)
                    _processedAlerts.RemoveAt(0);

                return alert;
            }

            return null;
        }
    }

    public List<Alert> GetProcessedAlerts()
    {
        lock (_lock)
        {
            return _processedAlerts
                .OrderByDescending(a => a.Timestamp)
                .ToList();
        }
    }


    public object GetStatistics()
    {
        lock (_lock)
        {
            return new
            {
                Pending = _queue.Count,
                TotalEnqueued = _totalEnqueued,
                TotalProcessed = _totalProcessed,
                TotalDiscarded = _totalDiscarded,
                CriticalProcessed = _processedAlerts.Count(
                    a => a.Severity == AnomalySeverity.Critical),
                WarningProcessed = _processedAlerts.Count(
                    a => a.Severity == AnomalySeverity.Warning)
            };
        }
    }
}
