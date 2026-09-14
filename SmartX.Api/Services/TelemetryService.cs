using SmartX.Api.DataStructures;
using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class TelemetryService
{
    private readonly HistoricalTelemetryBuffer _buffer;
    private readonly SensorService _sensorService;

    public TelemetryService(SensorService sensorService)
    {
        _sensorService = sensorService;
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

        _buffer.Add(reading);
        _sensorService.UpdateLastSeen(packet.DeviceId);
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

        _buffer.Add(reading);
        _sensorService.UpdateLastSeen(packet.DeviceId);
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

        _buffer.Add(reading);
        _sensorService.UpdateLastSeen(packet.DeviceId);
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
