using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/device-states")]
public class DeviceStatesController : ControllerBase
{
    private readonly DeviceStateService _deviceStates;

    public DeviceStatesController(DeviceStateService deviceStates)
    {
        _deviceStates = deviceStates;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_deviceStates.GetAllStates());
    }

    [HttpGet("count")]
    public IActionResult GetCount()
    {
        return Ok(new
        {
            DeviceCount = _deviceStates.GetDeviceCount()
        });
    }

    [HttpGet("{deviceId}")]
    public IActionResult GetByDeviceId(string deviceId)
    {
        var state = _deviceStates.GetState(deviceId);

        if (state is null)
            return NotFound(new
            {
                Message = "Device state not found."
            });

        return Ok(state);
    }
}
