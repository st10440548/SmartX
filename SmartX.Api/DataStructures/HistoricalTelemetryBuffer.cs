using SmartX.Api.Models;

namespace SmartX.Api.DataStructures;

public class HistoricalTelemetryBuffer
{
    private readonly TelemetryReading[][] _batches;
    private readonly int _batchSize;
    private int _currentBatch;
    private int _currentIndex;
    private int _count;

    public HistoricalTelemetryBuffer(int batchCount = 20, int batchSize = 500)
    {
        _batchSize = batchSize;
        _batches = new TelemetryReading[batchCount][];

        for (int i = 0; i < batchCount; i++)
        {
            _batches[i] = new TelemetryReading[batchSize];
        }
    }

    public int Count => _count;

    public int Capacity => _batches.Length * _batchSize;

    public void Add(TelemetryReading reading)
    {
        _batches[_currentBatch][_currentIndex] = reading;

        _currentIndex++;

        if (_currentIndex >= _batchSize)
        {
            _currentIndex = 0;
            _currentBatch = (_currentBatch + 1) % _batches.Length;
        }

        if (_count < Capacity)
        {
            _count++;
        }
    }

    public List<TelemetryReading> GetAll()
    {
        var result = new List<TelemetryReading>(_count);

        for (int batch = 0; batch < _batches.Length; batch++)
        {
            for (int index = 0; index < _batchSize; index++)
            {
                var reading = _batches[batch][index];

                if (reading is not null)
                {
                    result.Add(reading);
                }
            }
        }

        return result
            .OrderByDescending(x => x.Timestamp)
            .ToList();
    }

    public List<TelemetryReading> GetForDevice(string deviceId)
    {
        return GetAll()
            .Where(x =>
                x.DeviceId.Equals(
                    deviceId,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<TelemetryReading> GetAnomalies()
    {
        return GetAll()
            .Where(x => x.IsAnomaly)
            .ToList();
    }
}
