using SmartX.Api.DataStructures;
using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class TelemetryQueueService
{
    private readonly Queue<TelemetryPacket> _queue = new();
    private readonly object _lock = new();

    private long _totalEnqueued;
    private long _totalProcessed;

    public void Enqueue(TelemetryReading reading)
    {
        lock (_lock)
        {
            _queue.Enqueue(new TelemetryPacket
            {
                DeviceId = reading.DeviceId,
                Timestamp = reading.Timestamp,
                Location = reading.Location,
                SensorCategory = reading.Category.ToString(),
                Unit = reading.Unit,
                Value = reading
            });
            _totalEnqueued++;
        }
    }

    public bool TryDequeue(out TelemetryReading? reading)
    {
        lock (_lock)
        {
            if (_queue.Count == 0)
            {
                reading = null;
                return false;
            }

            var packet = _queue.Dequeue();
            reading = packet.Value as TelemetryReading;

            if (reading is null)
                throw new InvalidOperationException(
                    "Telemetry queue contained an invalid reading.");

            _totalProcessed++;
            return true;
        }
    }

    public QueueStatistics GetStatistics()
    {
        lock (_lock)
        {
            return new QueueStatistics
            {
                Pending = _queue.Count,
                TotalEnqueued = _totalEnqueued,
                TotalProcessed = _totalProcessed
            };
        }
    }
}

public class QueueStatistics
{
    public int Pending { get; set; }
    public long TotalEnqueued { get; set; }
    public long TotalProcessed { get; set; }
}
