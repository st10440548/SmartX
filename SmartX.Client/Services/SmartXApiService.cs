using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SmartX.Client.Services;

public class SmartXApiService
{
    private readonly HttpClient _http;

    public SmartXApiService(HttpClient http)
    {
        _http = http;
    }


    public async Task<List<PredictiveRecommendationDto>> GetRecommendationsAsync()
    {
        return await _http.GetFromJsonAsync<List<PredictiveRecommendationDto>>(
            "api/recommendations")
            ?? new List<PredictiveRecommendationDto>();
    }

    public async Task<List<SensorDto>> GetSensorsAsync()
    {
        return await _http.GetFromJsonAsync<List<SensorDto>>(
            "api/sensors")
            ?? new List<SensorDto>();
    }

    public async Task<SensorDto?> RegisterSensorAsync(
        CreateSensorDto sensor)
    {
        var response = await _http.PostAsJsonAsync(
            "api/sensors",
            sensor);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<SensorDto>();
    }

    public async Task<SensorDto?> GetSensorByDeviceIdAsync(
        string deviceId)
    {
        return await _http.GetFromJsonAsync<SensorDto>(
            $"api/sensors/device/{Uri.EscapeDataString(deviceId)}");
    }

    public async Task<List<TelemetryReadingDto>> GetTelemetryAsync()
    {
        return await _http.GetFromJsonAsync<List<TelemetryReadingDto>>(
            "api/telemetry")
            ?? new List<TelemetryReadingDto>();
    }

    public async Task<List<TelemetryReadingDto>> GetDeviceTelemetryAsync(
        string deviceId)
    {
        return await _http.GetFromJsonAsync<List<TelemetryReadingDto>>(
            $"api/telemetry/{Uri.EscapeDataString(deviceId)}")
            ?? new List<TelemetryReadingDto>();
    }

    public async Task<List<TelemetryReadingDto>> GetAnomaliesAsync()
    {
        return await _http.GetFromJsonAsync<List<TelemetryReadingDto>>(
            "api/telemetry/anomalies")
            ?? new List<TelemetryReadingDto>();
    }

    public async Task<List<MeshNodeDto>> GetMeshNodesAsync()
    {
        return await _http.GetFromJsonAsync<List<MeshNodeDto>>(
            "api/mesh")
            ?? new List<MeshNodeDto>();
    }

    public async Task<MeshNodeDto?> GetMeshNodeAsync(
        string nodeId)
    {
        return await _http.GetFromJsonAsync<MeshNodeDto>(
            $"api/mesh/{Uri.EscapeDataString(nodeId)}");
    }

    public async Task<List<MeshNodeDto>> GetMeshChildrenAsync(
        string nodeId)
    {
        return await _http.GetFromJsonAsync<List<MeshNodeDto>>(
            $"api/mesh/{Uri.EscapeDataString(nodeId)}/children")
            ?? new List<MeshNodeDto>();
    }

    public async Task<List<MeshNodeDto>> GetMeshRouteAsync(
        string nodeId)
    {
        return await _http.GetFromJsonAsync<List<MeshNodeDto>>(
            $"api/mesh/{Uri.EscapeDataString(nodeId)}/route")
            ?? new List<MeshNodeDto>();
    }

    public async Task<MeshHealthDto?> GetMeshHealthAsync()
    {
        return await _http.GetFromJsonAsync<MeshHealthDto>(
            "api/mesh/health");
    }

    public async Task<List<DeviceCommandDto>> GetCommandsAsync()
    {
        return await _http.GetFromJsonAsync<List<DeviceCommandDto>>(
            "api/commands")
            ?? new List<DeviceCommandDto>();
    }

    public async Task<List<DeviceCommandDto>> GetDeviceCommandsAsync(
        string deviceId)
    {
        return await _http.GetFromJsonAsync<List<DeviceCommandDto>>(
            $"api/commands/{Uri.EscapeDataString(deviceId)}")
            ?? new List<DeviceCommandDto>();
    }

    public async Task<DeviceCommandDto?> SendCommandAsync(
        CreateCommandDto command)
    {
        var response = await _http.PostAsJsonAsync(
            "api/commands",
            command);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<DeviceCommandDto>();
    }

    public async Task<DeviceCommandDto?> UndoLastCommandAsync()
    {
        using var response = await _http.PostAsync(
            "api/commands/undo",
            null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<DeviceCommandDto>();
    }

    public async Task<List<SensorAttachmentDto>> GetAttachmentsAsync(
        string deviceId)
    {
        return await _http.GetFromJsonAsync<List<SensorAttachmentDto>>(
            $"api/attachments/{Uri.EscapeDataString(deviceId)}")
            ?? new List<SensorAttachmentDto>();
    }

    public async Task<SensorAttachmentDto?> UploadAttachmentAsync(
        string deviceId,
        Stream fileStream,
        string fileName,
        string contentType)
    {
        using var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(fileStream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(contentType);

        content.Add(
            fileContent,
            "file",
            fileName);

        var response = await _http.PostAsync(
            $"api/attachments/{Uri.EscapeDataString(deviceId)}",
            content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<SensorAttachmentDto>();
    }
}

public class SensorDto
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DeploymentLocation { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public int Category { get; set; }
    public string DataType { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TelemetryReadingDto
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Category { get; set; }
    public string DataType { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public object? Value { get; set; }
    public bool IsAnomaly { get; set; }
    public int Severity { get; set; }
}

public class MeshNodeDto
{
    public string NodeId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NodeType { get; set; } = string.Empty;
    public string ParentNodeId { get; set; } = string.Empty;
    public List<string> ChildNodeIds { get; set; } = new();
    public int SignalStrength { get; set; }
    public int HopCount { get; set; }
    public bool IsOnline { get; set; }
    public string RouteStatus { get; set; } = string.Empty;
}

public class MeshHealthDto
{
    public int TotalNodes { get; set; }
    public int HealthyNodes { get; set; }
    public int DegradedNodes { get; set; }
}

public class DeviceCommandDto
{
    public Guid Id { get; set; }

    public string DeviceId { get; set; } = string.Empty;

    public string CommandType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ExecutedAt { get; set; }
}

public class CreateCommandDto
{
    public string DeviceId { get; set; } = string.Empty;

    public string CommandType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;
}

public class SensorAttachmentDto
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class CreateSensorDto
{
    public string DeviceId { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DeploymentLocation { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public int Category { get; set; }
    public string DataType { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
}


public class PredictiveRecommendationDto
{
    public string DeviceId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string RecommendedAction { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
