# Smart-X Part 2: Testing and Demonstration Guide

## 1. Project Overview

Smart-X is an IoT mesh ecosystem developed using ASP.NET Core,
Blazor WebAssembly, C#, and Docker.

Part 2 demonstrates data structure implementations, telemetry
processing, alert management, command history, and automated testing.

## 2. Running the Application

From the project root, execute:

```bash
docker compose up -d --build
```

Open the following addresses:

- Frontend: http://localhost:5170
- API health: http://localhost:5137/health
- Sensor API: http://localhost:5137/api/sensors

Stop the application using:

```bash
docker compose down
```

## 3. Docker Health Verification

Run:

```bash
docker compose ps
```

The API container should display a healthy status.

The health endpoint should return HTTP 200 with the response
"Healthy".

This confirms that the API responds to HTTP requests. It does
not independently verify every background service.

## 4. Automated Testing

Run all automated tests:

```bash
dotnet test SmartX.Tests/SmartX.Tests.csproj
```

The test suite currently contains 35 tests covering:

- FIFO telemetry queue ordering and statistics
- Concurrent telemetry producers and consumers
- Device state dictionary operations
- Priority-based alert processing
- Active alert duplicate prevention
- Safe alert resolution
- Alert processing statistics and history limits
- LIFO command undo operations
- Command history consistency
- Chronological telemetry history
- Telemetry retention limits

The expected result is 35 passing tests and zero failures.

## 5. Data Structure Demonstration

### FIFO Queue

Explain that the first telemetry reading added is the first
reading removed.

Show the telemetry queue tests and processing statistics.

### Priority Queue

Explain that critical alerts are prioritised over warning
alerts when both are pending.

Show the priority alert tests and processing statistics.

### Dictionary

Explain how device identifiers are used as keys to retrieve
and update the latest device states.

Show the device state tests.

### HashSet

Explain how active alert keys prevent duplicate alerts
for the same device and severity.

Show the active alert tests and alert resolution tests.

### Stack (LIFO)

Explain that the most recent reversible actuator command
is undone first.

Show the command undo and command history tests.

### SortedDictionary

Explain how telemetry records are organised by timestamp
for chronological retrieval.

Show the chronological telemetry and retention tests.

## 6. Continuous Integration

The GitHub Actions workflow is located at:

.github/workflows/dotnet-ci.yml

On pushes and pull requests targeting main, the workflow
restores dependencies, builds the solution, and runs tests.

Review the Actions tab on GitHub to confirm successful runs.

## 7. Suggested Demonstration Order

1. Show the project structure and Git commit history.
2. Start the application using Docker Compose.
3. Verify the API container is healthy.
4. Open the Blazor frontend.
5. Show sensor data and telemetry functionality.
6. Demonstrate alert processing and device states.
7. Explain the LIFO command undo feature.
8. Run the automated test suite.
9. Show the GitHub Actions workflow.
10. Explain the six data structures and their purpose.

## 8. Final Verification Checklist

- [ ] Docker images build successfully
- [ ] API container reports healthy
- [ ] Frontend opens successfully
- [ ] API health endpoint returns HTTP 200
- [ ] All 35 automated tests pass
- [ ] GitHub Actions workflow passes
- [ ] At least 20 new Part 2 commits are present
- [ ] All final changes are pushed to GitHub
