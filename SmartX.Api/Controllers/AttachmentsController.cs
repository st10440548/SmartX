using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly FileStorageService _fileStorageService;
    private readonly SensorService _sensorService;
    private readonly AttachmentService _attachmentService;

    public AttachmentsController(
        FileStorageService fileStorageService,
        SensorService sensorService,
        AttachmentService attachmentService)
    {
        _fileStorageService = fileStorageService;
        _sensorService = sensorService;
        _attachmentService = attachmentService;
    }

    [HttpGet("{deviceId}")]
    public IActionResult GetAttachments(string deviceId)
    {
        var sensor =
            _sensorService.GetByDeviceId(deviceId);

        if (sensor is null)
        {
            return NotFound(new
            {
                message = "Sensor not found."
            });
        }

        return Ok(
            _attachmentService.GetForDevice(deviceId));
    }

    [HttpPost("{deviceId}")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Upload(
        string deviceId,
        IFormFile file)
    {
        var sensor =
            _sensorService.GetByDeviceId(deviceId);

        if (sensor is null)
        {
            return NotFound(new
            {
                message = "Sensor not found."
            });
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "A file is required."
            });
        }

        try
        {
            var attachment =
                await _fileStorageService.SaveAsync(
                    deviceId,
                    file);

            _attachmentService.Add(attachment);

            return Ok(attachment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}
