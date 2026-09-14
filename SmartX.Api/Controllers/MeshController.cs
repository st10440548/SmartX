using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeshController : ControllerBase
{
    private readonly MeshTopologyService _meshService;

    public MeshController(MeshTopologyService meshService)
    {
        _meshService = meshService;
    }

    [HttpGet]
    public IActionResult GetTopology()
    {
        return Ok(_meshService.GetAll());
    }

    [HttpGet("{nodeId}")]
    public IActionResult GetNode(string nodeId)
    {
        var node = _meshService.GetByNodeId(nodeId);

        if (node is null)
        {
            return NotFound(new
            {
                message = "Mesh node not found."
            });
        }

        return Ok(node);
    }

    [HttpGet("{nodeId}/children")]
    public IActionResult GetChildren(string nodeId)
    {
        var node = _meshService.GetByNodeId(nodeId);

        if (node is null)
        {
            return NotFound(new
            {
                message = "Mesh node not found."
            });
        }

        return Ok(_meshService.GetChildren(nodeId));
    }

    [HttpGet("{nodeId}/route")]
    public IActionResult GetRoute(string nodeId)
    {
        var node = _meshService.GetByNodeId(nodeId);

        if (node is null)
        {
            return NotFound(new
            {
                message = "Mesh node not found."
            });
        }

        return Ok(_meshService.GetRouteToGateway(nodeId));
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            totalNodes = _meshService.GetAll().Count,
            healthyNodes = _meshService.GetHealthyNodeCount(),
            degradedNodes = _meshService.GetDegradedNodeCount()
        });
    }
}
