using System;
using System.Collections.Generic;

namespace SmartX.Api.Services;

public class ActuatorStateService
{
    private readonly Dictionary<string, bool> _states =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly object _lock = new();

    public bool GetState(string deviceId)
    {
        lock (_lock)
        {
            return _states.TryGetValue(deviceId, out var state)
                && state;
        }
    }

    public bool SetState(string deviceId, bool newState)
    {
        lock (_lock)
        {
            var previousState = GetState(deviceId);
            _states[deviceId] = newState;
            return previousState;
        }
    }
}
