# Smart-X IoT Mesh Ecosystem

Smart-X is a .NET 10 IoT monitoring application with an
ASP.NET Core Web API and a Blazor WebAssembly frontend.

## Features

- Simulated IoT sensor telemetry
- Real-time dashboard with category and severity filters
- FIFO telemetry processing
- Priority-based anomaly alert processing
- Active-alert duplicate prevention
- Device-state tracking
- Chronological telemetry history
- Device command history and undo
- Predictive recommendations
- Mesh topology API

## Technology Stack

- C# / .NET 10
- ASP.NET Core Web API
- Blazor WebAssembly
- Docker and Docker Compose
- Nginx

## Requirements

Install either:

- Docker Desktop with Docker Compose

Or, for local development:

- .NET 10 SDK

## Run with Docker

From the repository root:

```bash
docker compose up -d --build
```

Open:

- Frontend: http://localhost:5170
- API: http://localhost:5137

Check container status:

```bash
docker compose ps
```

View API logs:

```bash
docker compose logs api
```

Stop the application:

```bash
docker compose down
```

## Run Without Docker

Open two terminals in the repository root.

Terminal 1:

```bash
dotnet run --project SmartX.Api
```

Terminal 2:

```bash
dotnet run --project SmartX.Client
```

The API normally runs on port 5137.
The frontend uses the API at http://localhost:5137.

## Build the Solution

```bash
dotnet restore SmartX.slnx
dotnet build SmartX.slnx
```

## API Verification

With the API running, test:

```bash
curl http://localhost:5137/api/sensors
curl http://localhost:5137/api/telemetry-queue/statistics
curl http://localhost:5137/api/alerts/statistics
curl http://localhost:5137/api/device-states/count
curl http://localhost:5137/api/chronological-telemetry/statistics
curl http://localhost:5137/api/recommendations
```

These endpoints help verify sensor simulation, telemetry
processing, alerts, device state, historical records,
and recommendations.

## Data Structures

The Part 2 implementation uses:

| Data Structure | Purpose |
|---|---|
| Queue<TelemetryPacket> | FIFO telemetry ingestion |
| PriorityQueue<Alert, int> | Critical alerts before warnings |
| Stack<CommandRecord> | Last-in-first-out command undo |
| Dictionary<string, DeviceState> | Latest device state lookup |
| SortedDictionary<DateTime, List<TelemetryRecord>> | Chronological telemetry |
| HashSet<string> | Active-alert duplicate prevention |

## Notes

Telemetry and command state are maintained in memory.
Restarting the API resets the current simulated state.

The network topology module is included as part of
the broader Smart-X ecosystem.

## Testing the FIFO Telemetry Queue

The API uses `Queue<TelemetryPacket>` to process incoming
sensor readings in first-in-first-out order.

Start the application:

```bash
docker compose up -d
```

Check the queue statistics:

```bash
curl http://localhost:5137/api/telemetry-queue/statistics
```

The response contains:

- `totalEnqueued`: Number of telemetry packets received
- `totalProcessed`: Number of packets processed
- `pending`: Packets waiting to be processed

Wait approximately 10 seconds and request the statistics again.

Expected behaviour:

1. `totalEnqueued` increases as the simulator generates readings.
2. `totalProcessed` increases as packets are processed.
3. `pending` should normally remain low because processing is continuous.
4. Queue accounting should remain consistent:

   `totalEnqueued = totalProcessed + pending`

These checks verify ingestion and processing counts. A separate
controlled test is needed to prove FIFO ordering directly.

## Testing the Device-State Dictionary

The API uses `Dictionary<string, DeviceState>` to maintain
the latest state of each simulated device.

Retrieve the number of tracked devices:

```bash
curl http://localhost:5137/api/device-states/count
```

Expected result: approximately 100 devices after simulator startup.

Retrieve the complete dictionary-backed device-state list:

```bash
curl http://localhost:5137/api/device-states
```

Retrieve one device:

```bash
curl http://localhost:5137/api/device-states/ESP32-0002
```

Expected behaviour:

1. The requested device is returned using its device ID.
2. The response includes its latest value and timestamp.
3. Repeating the request after a few seconds should show
   a newer timestamp as telemetry updates the device.
4. Actuator values should reflect commands sent through
   the command API.

The dictionary provides direct lookup of the latest known
state without scanning the complete telemetry history.

## Testing Priority Alert Processing

Retrieve alert statistics:

```bash
curl http://localhost:5137/api/alerts/statistics
```

Wait 10 seconds and repeat the request.

Expected behaviour:

- New anomalies increase `totalEnqueued`.
- The background worker increases `totalProcessed`.
- Resolved alerts may increase `totalDiscarded`.
- Critical alerts have higher processing priority than warnings.

Queue accounting:

`totalEnqueued = totalProcessed + totalDiscarded + pending`

The counters verify processing activity, while a controlled
test is required to prove priority ordering directly.
