using SmartX.Api.DataStructures;
using SmartX.Api.Services;

namespace SmartX.Api.Services;

public class TelemetrySimulator : BackgroundService
{
    private readonly TelemetryService _telemetryService;
    private readonly SensorService _sensorService;
    private readonly Random _random = new();

    private readonly Dictionary<string, DateTime> _disconnectUntil = new();

    public TelemetrySimulator(
        TelemetryService telemetryService,
        SensorService sensorService)
    {
        _telemetryService = telemetryService;
        _sensorService = sensorService;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            GenerateTelemetryBatch();

            await Task.Delay(
                TimeSpan.FromSeconds(2),
                stoppingToken);
        }
    }

    private void GenerateTelemetryBatch()
    {
        var sensors = _sensorService.GetAll();

        foreach (var sensor in sensors)
        {
            if (ShouldDisconnect(sensor.DeviceId))
            {
                continue;
            }

            switch (sensor.Category)
            {
                case Models.SensorCategory.Environmental:
                    GenerateTemperature(sensor);
                    break;

                case Models.SensorCategory.PowerConsumption:
                    GeneratePower(sensor);
                    break;

                case Models.SensorCategory.Actuator:
                    GenerateActuator(sensor);
                    break;
            }
        }

        _sensorService.RefreshOnlineStatus(
            TimeSpan.FromSeconds(10));
    }

    private bool ShouldDisconnect(string deviceId)
    {
        var now = DateTime.UtcNow;

        if (_disconnectUntil.TryGetValue(deviceId, out var reconnectTime))
        {
            if (now < reconnectTime)
            {
                return true;
            }

            _disconnectUntil.Remove(deviceId);
            return false;
        }

        // Roughly 1% chance of starting a simulated disconnect.
        if (_random.NextDouble() < 0.01)
        {
            // Keep the device disconnected for 12–20 seconds.
            var seconds = _random.Next(12, 21);

            _disconnectUntil[deviceId] =
                now.AddSeconds(seconds);

            return true;
        }

        return false;
    }

    private void GenerateTemperature(Models.Sensor sensor)
    {
        var temperature =
            18f + ((float)_random.NextDouble() * 12f);

        if (_random.NextDouble() < 0.02)
        {
            temperature =
                _random.Next(45, 61);
        }

        var packet = new TelemetryPacket<float>
        {
            DeviceId = sensor.DeviceId,
            Timestamp = DateTime.UtcNow,
            Location = sensor.DeploymentLocation,
            SensorCategory = "Environmental",
            Unit = "°C",
            Value = temperature
        };

        _telemetryService.AddTemperature(packet);
    }

    private void GeneratePower(Models.Sensor sensor)
    {
        var power =
            _random.Next(200, 3000);

        if (_random.NextDouble() < 0.02)
        {
            power =
                _random.Next(5500, 8000);
        }

        var packet = new TelemetryPacket<int>
        {
            DeviceId = sensor.DeviceId,
            Timestamp = DateTime.UtcNow,
            Location = sensor.DeploymentLocation,
            SensorCategory = "PowerConsumption",
            Unit = "W",
            Value = power
        };

        _telemetryService.AddPower(packet);
    }

    private void GenerateActuator(Models.Sensor sensor)
    {
        var state =
            _random.Next(0, 2) == 1;

        var packet = new TelemetryPacket<bool>
        {
            DeviceId = sensor.DeviceId,
            Timestamp = DateTime.UtcNow,
            Location = sensor.DeploymentLocation,
            SensorCategory = "Actuator",
            Unit = "State",
            Value = state
        };

        _telemetryService.AddActuator(packet);
    }
}
