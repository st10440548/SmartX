using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class SensorService
{
    private readonly List<Sensor> _sensors = new();
    private readonly object _lock = new();

    public SensorService()
    {
        SeedSensors();
    }

    public List<Sensor> GetAll()
    {
        lock (_lock)
        {
            return _sensors
                .OrderBy(x => x.Name)
                .ToList();
        }
    }

    public Sensor? GetById(Guid id)
    {
        lock (_lock)
        {
            return _sensors.FirstOrDefault(x => x.Id == id);
        }
    }

    public Sensor? GetByDeviceId(string deviceId)
    {
        lock (_lock)
        {
            return _sensors.FirstOrDefault(x =>
                x.DeviceId.Equals(
                    deviceId,
                    StringComparison.OrdinalIgnoreCase));
        }
    }

    public Sensor RegisterSensor(Sensor sensor)
    {
        lock (_lock)
        {
            if (_sensors.Any(x =>
                x.DeviceId.Equals(
                    sensor.DeviceId,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"A sensor with device ID '{sensor.DeviceId}' already exists.");
            }

            sensor.Id = Guid.NewGuid();
            sensor.CreatedAt = DateTime.UtcNow;
            sensor.LastSeen = DateTime.UtcNow;
            sensor.IsOnline = true;

            _sensors.Add(sensor);

            return sensor;
        }
    }

    public bool UpdateLastSeen(string deviceId)
    {
        lock (_lock)
        {
            var sensor = _sensors.FirstOrDefault(x =>
                x.DeviceId.Equals(
                    deviceId,
                    StringComparison.OrdinalIgnoreCase));

            if (sensor is null)
            {
                return false;
            }

            sensor.LastSeen = DateTime.UtcNow;
            sensor.IsOnline = true;

            return true;
        }
    }

    public void RefreshOnlineStatus(TimeSpan timeout)
    {
        lock (_lock)
        {
            var cutoff = DateTime.UtcNow - timeout;

            foreach (var sensor in _sensors)
            {
                sensor.IsOnline = sensor.LastSeen >= cutoff;
            }
        }
    }

    private void SeedSensors()
    {
        var random = new Random(42);

        for (int i = 1; i <= 100; i++)
        {
            var category = (i % 3) switch
            {
                0 => SensorCategory.Environmental,
                1 => SensorCategory.PowerConsumption,
                _ => SensorCategory.Actuator
            };

            var sensor = new Sensor
            {
                DeviceId = $"ESP32-{i:0000}",

                MacAddress =
                    $"AA:BB:CC:{i / 256:X2}:{i % 256:X2}:{random.Next(0, 256):X2}",

                Name = $"Smart-X Sensor {i:0000}",

                DeploymentLocation =
                    $"Facility {((i - 1) / 25) + 1}",

                Zone =
                    $"Zone {((i - 1) / 10) + 1}",

                NodeId =
                    $"NODE-{i:000}",

                Category = category,

                DataType = category switch
                {
                    SensorCategory.Environmental => "Float",
                    SensorCategory.PowerConsumption => "Integer",
                    SensorCategory.Actuator => "Boolean",
                    _ => "Unknown"
                },

                Unit = category switch
                {
                    SensorCategory.Environmental => "°C",
                    SensorCategory.PowerConsumption => "W",
                    SensorCategory.Actuator => "State",
                    _ => string.Empty
                },

                IsOnline = true,
                LastSeen = DateTime.UtcNow
            };

            _sensors.Add(sensor);
        }
    }
}
