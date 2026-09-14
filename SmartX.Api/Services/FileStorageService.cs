using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class FileStorageService
{
    private readonly string _uploadDirectory;

    public FileStorageService(IWebHostEnvironment environment)
    {
        _uploadDirectory = Path.Combine(
            environment.ContentRootPath,
            "uploads");

        Directory.CreateDirectory(_uploadDirectory);
    }

    public async Task<SensorAttachment> SaveAsync(
        string deviceId,
        IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new InvalidOperationException(
                "The uploaded file is empty.");
        }

        var extension = Path.GetExtension(file.FileName);

        var storedFileName =
            $"{Guid.NewGuid():N}{extension}";

        var filePath = Path.Combine(
            _uploadDirectory,
            storedFileName);

        await using var stream =
            new FileStream(
                filePath,
                FileMode.CreateNew);

        await file.CopyToAsync(stream);

        return new SensorAttachment
        {
            DeviceId = deviceId,
            FileName = Path.GetFileName(file.FileName),
            StoredFileName = storedFileName,
            ContentType =
                string.IsNullOrWhiteSpace(file.ContentType)
                    ? "application/octet-stream"
                    : file.ContentType,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        };
    }
}
