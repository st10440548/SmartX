using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class AttachmentService
{
    private readonly List<SensorAttachment> _attachments = new();
    private readonly object _lock = new();

    public void Add(SensorAttachment attachment)
    {
        lock (_lock)
        {
            _attachments.Add(attachment);
        }
    }

    public List<SensorAttachment> GetForDevice(string deviceId)
    {
        lock (_lock)
        {
            return _attachments
                .Where(x =>
                    x.DeviceId.Equals(
                        deviceId,
                        StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.UploadedAt)
                .ToList();
        }
    }
}
