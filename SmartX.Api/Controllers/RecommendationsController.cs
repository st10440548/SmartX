using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/recommendations")]
public class RecommendationsController : ControllerBase
{
    private readonly PredictiveRecommendationService _recommendations;

    public RecommendationsController(
        PredictiveRecommendationService recommendations)
    {
        _recommendations = recommendations;
    }

    [HttpGet]
    public IActionResult GetRecommendations()
    {
        return Ok(_recommendations.GetRecommendations());
    }

    [HttpGet("count")]
    public IActionResult GetCount()
    {
        var recommendations = _recommendations.GetRecommendations();

        return Ok(new
        {
            Total = recommendations.Count,
            Critical = recommendations.Count(
                r => r.Severity == "Critical"),
            Warning = recommendations.Count(
                r => r.Severity == "Warning")
        });
    }
}
