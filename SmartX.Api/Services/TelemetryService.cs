using SmartX.Api.DataStructures;
using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class TelemetryService
{
    private readonly HistoricalTelemetryBuffer _buffer;
    private readonly SensorService _sensorService;

    private readonly TelemetryQueueService _telemetryQueue;
    private readonly PriorityAlertService _priorityAlerts;
    private readonly ActiveAlertService _activeAlerts;
    private readonly DeviceStateService _deviceStates;
    private readonly ChronologicalTelemetryService _chronologicalHistory;
    private readonly object _processingLock = new();

    public TelemetryService(
        SensorService sensorService,
        TelemetryQueueService telemetryQueue,
        PriorityAlertService priorityAlerts,
        ActiveAlertService activeAlerts,
        DeviceStateService deviceStates,
        ChronologicalTelemetryService chronologicalHistory)
    {
        _sensorService = sensorService;
        _telemetryQueue = telemetryQueue;
        _priorityAlerts = priorityAlerts;
        _activeAlerts = activeAlerts;
        _deviceStates = deviceStates;
        _chronologicalHistory = chronologicalHistory;
        _buffer = new HistoricalTelemetryBuffer();
    }

    public void AddTemperature(TelemetryPacket<float> packet)
    {
        var severity = packet.Value switch
        {
            < 0 or > 50 => AnomalySeverity.Critical,
            < 5 or > 40 => AnomalySeverity.Warning,
            _ => AnomalySeverity.None
        };

        var reading = new TelemetryReading
        {
            DeviceId = packet.DeviceId,
            Timestamp = packet.Timestamp,
            Location = packet.Location,
            Category = SensorCategory.Environmental,
            DataType = "float",
            Unit = packet.Unit,
            Value = packet.Value,
            IsAnomaly = severity != AnomalySeverity.None,
            Severity = severity
        };

        _telemetryQueue.Enqueue(reading);
        ProcessPendingTelemetry();
    }

    public void AddPower(TelemetryPacket<int> packet)
    {
        var severity = packet.Value switch
        {
            > 7500 => AnomalySeverity.Critical,
            > 5000 => AnomalySeverity.Warning,
            _ => AnomalySeverity.None
        };

        var reading = new TelemetryReading
        {
            DeviceId = packet.DeviceId,
            Timestamp = packet.Timestamp,
            Location = packet.Location,
            Category = SensorCategory.PowerConsumption,
            DataType = "int",
            Unit = packet.Unit,
            Value = packet.Value,
            IsAnomaly = severity != AnomalySeverity.None,
            Severity = severity
        };

        _telemetryQueue.Enqueue(reading);
        ProcessPendingTelemetry();
    }

    public void AddActuator(TelemetryPacket<bool> packet)
    {
        var reading = new TelemetryReading
        {
            DeviceId = packet.DeviceId,
            Timestamp = packet.Timestamp,
            Location = packet.Location,
            Category = SensorCategory.Actuator,
            DataType = "bool",
            Unit = packet.Unit,
            Value = packet.Value,
            IsAnomaly = false,
            Severity = AnomalySeverity.None
        };

        _telemetryQueue.Enqueue(reading);
        ProcessPendingTelemetry();
    }

    private void ProcessPendingTelemetry()
    {
        lock (_processingLock)
        {
            while (_telemetryQueue.TryDequeue(out var reading))
            {
                if (reading is null)
                    continue;

                _buffer.Add(reading);
                _sensorService.UpdateLastSeen(reading.DeviceId);
                _deviceStates.UpdateState(reading);
                _chronologicalHistory.AddReading(reading);

                if (reading.IsAnomaly)
                {
                    var alert = new Alert
                    {
                        DeviceId = reading.DeviceId,
                        Severity = reading.Severity,
                        Timestamp = reading.Timestamp,
                        Message = $"{reading.Category} anomaly: {reading.Value} {reading.Unit}"
                    };

                    // A device has one active severity at a time.
                    var otherSeverity = reading.Severity == AnomalySeverity.Critical
                        ? AnomalySeverity.Warning
                        : AnomalySeverity.Critical;

                    _activeAlerts.Resolve(new Alert
                    {
                        DeviceId = reading.DeviceId,
                        Severity = otherSeverity
                    });

                    if (_activeAlerts.TryActivate(alert))
                        _priorityAlerts.Enqueue(alert);
                }
                else
                {
                    // A normal reading clears active anomalies.
                    foreach (var severity in new[]
                    {
                        AnomalySeverity.Warning,
                        AnomalySeverity.Critical
                    })
                    {
                        _activeAlerts.Resolve(new Alert
                        {
                            DeviceId = reading.DeviceId,
                            Severity = severity
                        });
                    }
                }
            }
        }
    }

    public List<TelemetryReading> GetAll()
    {
        return _buffer.GetAll();
    }

    public List<TelemetryReading> GetForDevice(string deviceId)
    {
        return _buffer.GetForDevice(deviceId);
    }

    public List<TelemetryReading> GetAnomalies()
    {
        return _buffer.GetAnomalies();
    }
}
