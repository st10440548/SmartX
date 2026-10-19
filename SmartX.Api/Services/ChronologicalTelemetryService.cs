using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class ChronologicalTelemetryService
{
    private readonly SortedDictionary<DateTime, List<TelemetryRecord>>
        _history = new();

    private readonly object _lock = new();

    private long _totalRecords;
    private const int MaxHistoryRecords = 10000;
    private int _storedRecords;

    public void AddReading(TelemetryReading reading)
    {
        lock (_lock)
        {
            if (!_history.TryGetValue(reading.Timestamp, out var records))
            {
                records = new List<TelemetryRecord>();
                _history[reading.Timestamp] = records;
            }

            records.Add(new TelemetryRecord
            {
                DeviceId = reading.DeviceId,
                Timestamp = reading.Timestamp,
                Category = reading.Category,
                Value = reading.Value,
                Unit = reading.Unit,
                IsAnomaly = reading.IsAnomaly,
                Severity = reading.Severity
            });

            _totalRecords++;
            _storedRecords++;

            while (_storedRecords > MaxHistoryRecords)
            {
                var oldest = _history.First();
                oldest.Value.RemoveAt(0);
                _storedRecords--;

                if (oldest.Value.Count == 0)
                    _history.Remove(oldest.Key);
            }
        }
    }

    public List<TelemetryRecord> GetRecent(int count = 100)
    {
        lock (_lock)
        {
            return _history
                .Reverse()
                .SelectMany(entry => entry.Value)
                .OrderByDescending(record => record.Timestamp)
                .Take(Math.Clamp(count, 1, 1000))
                .ToList();
        }
    }

    public List<TelemetryRecord> GetForDevice(string deviceId)
    {
        lock (_lock)
        {
            return _history
                .Values
                .SelectMany(records => records)
                .Where(record => record.DeviceId.Equals(
                    deviceId, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public object GetStatistics()
    {
        lock (_lock)
        {
            return new
            {
                TotalRecords = _totalRecords,
                StoredRecords = _storedRecords,
                MaxHistoryRecords,
                UniqueTimestamps = _history.Count,
                OldestTimestamp = _history.Count > 0
                    ? _history.First().Key
                    : (DateTime?)null,
                NewestTimestamp = _history.Count > 0
                    ? _history.Last().Key
                    : (DateTime?)null
            };
        }
    }
}
