using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly SensorService _sensorService;

    public SensorsController(SensorService sensorService)
    {
        _sensorService = sensorService;
    }

    [HttpGet]
    public IActionResult GetSensors()
    {
        return Ok(_sensorService.GetAll());
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetSensor(Guid id)
    {
        var sensor = _sensorService.GetById(id);

        if (sensor is null)
        {
            return NotFound(new
            {
                message = "Sensor not found."
            });
        }

        return Ok(sensor);
    }

    [HttpGet("device/{deviceId}")]
    public IActionResult GetSensorByDeviceId(string deviceId)
    {
        var sensor = _sensorService.GetByDeviceId(deviceId);

        if (sensor is null)
        {
            return NotFound(new
            {
                message = "Sensor not found."
            });
        }

        return Ok(sensor);
    }

    [HttpPost]
    public IActionResult RegisterSensor([FromBody] Sensor sensor)
    {
        try
        {
            var registeredSensor = _sensorService.RegisterSensor(sensor);

            return CreatedAtAction(
                nameof(GetSensor),
                new { id = registeredSensor.Id },
                registeredSensor);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}
