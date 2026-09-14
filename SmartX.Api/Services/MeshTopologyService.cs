using SmartX.Api.Models;

namespace SmartX.Api.Services;

public class MeshTopologyService
{
    private readonly SensorService _sensorService;

    public MeshTopologyService(SensorService sensorService)
    {
        _sensorService = sensorService;
    }

    private List<MeshNode> BuildTopology()
    {
        var nodes = new List<MeshNode>();

        var sensors = _sensorService.GetAll();

        var gateway = new MeshNode
        {
            NodeId = "GATEWAY-01",
            DeviceId = "GATEWAY",
            Name = "Central Gateway",
            NodeType = "Gateway",
            ParentNodeId = string.Empty,
            SignalStrength = 100,
            HopCount = 0,
            IsOnline = true,
            RouteStatus = "Healthy"
        };

        nodes.Add(gateway);

        var sensorsByZone = sensors
            .GroupBy(sensor => sensor.Zone)
            .OrderBy(group => group.Key)
            .ToList();

        var zoneIndex = 1;

        foreach (var zoneGroup in sensorsByZone)
        {
            var routerId = $"ROUTER-{zoneIndex:00}";

            var router = new MeshNode
            {
                NodeId = routerId,
                DeviceId = routerId,
                Name = $"{zoneGroup.Key} Mesh Router",
                NodeType = "Router",
                ParentNodeId = gateway.NodeId,
                SignalStrength = 85 + (zoneIndex % 10),
                HopCount = 1,
                IsOnline = true,
                RouteStatus = "Healthy"
            };

            gateway.ChildNodeIds.Add(router.NodeId);
            nodes.Add(router);

            foreach (var sensor in zoneGroup)
            {
                var signal =
                    55 + Math.Abs(sensor.DeviceId.GetHashCode() % 40);

                var node = new MeshNode
                {
                    NodeId = sensor.NodeId,
                    DeviceId = sensor.DeviceId,
                    Name = sensor.Name,
                    NodeType = "Sensor",
                    ParentNodeId = router.NodeId,
                    SignalStrength = signal,
                    HopCount = 2,
                    IsOnline = sensor.IsOnline,
                    RouteStatus = signal < 65
                        ? "Degraded"
                        : "Healthy"
                };

                router.ChildNodeIds.Add(node.NodeId);
                nodes.Add(node);
            }

            zoneIndex++;
        }

        return nodes;
    }

    public List<MeshNode> GetAll()
    {
        return BuildTopology()
            .Select(CloneNode)
            .ToList();
    }

    public MeshNode? GetByNodeId(string nodeId)
    {
        var node = BuildTopology()
            .FirstOrDefault(x =>
                x.NodeId.Equals(
                    nodeId,
                    StringComparison.OrdinalIgnoreCase));

        return node is null ? null : CloneNode(node);
    }

    public List<MeshNode> GetChildren(string nodeId)
    {
        return BuildTopology()
            .Where(x =>
                x.ParentNodeId.Equals(
                    nodeId,
                    StringComparison.OrdinalIgnoreCase))
            .Select(CloneNode)
            .ToList();
    }

    public List<MeshNode> GetRouteToGateway(string nodeId)
    {
        var nodes = BuildTopology();
        var route = new List<MeshNode>();

        var current = nodes.FirstOrDefault(x =>
            x.NodeId.Equals(
                nodeId,
                StringComparison.OrdinalIgnoreCase));

        while (current is not null)
        {
            route.Add(CloneNode(current));

            if (string.IsNullOrWhiteSpace(current.ParentNodeId))
            {
                break;
            }

            current = nodes.FirstOrDefault(x =>
                x.NodeId.Equals(
                    current.ParentNodeId,
                    StringComparison.OrdinalIgnoreCase));
        }

        return route;
    }

    public int GetHealthyNodeCount()
    {
        return BuildTopology()
            .Count(x =>
                x.IsOnline &&
                x.RouteStatus == "Healthy");
    }

    public int GetDegradedNodeCount()
    {
        return BuildTopology()
            .Count(x =>
                x.RouteStatus == "Degraded");
    }

    private static MeshNode CloneNode(MeshNode node)
    {
        return new MeshNode
        {
            NodeId = node.NodeId,
            DeviceId = node.DeviceId,
            Name = node.Name,
            NodeType = node.NodeType,
            ParentNodeId = node.ParentNodeId,
            ChildNodeIds = node.ChildNodeIds.ToList(),
            SignalStrength = node.SignalStrength,
            HopCount = node.HopCount,
            IsOnline = node.IsOnline,
            RouteStatus = node.RouteStatus
        };
    }
}
