using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Discovery;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/discovery")]
public class DiscoveryController : ApiControllerBase
{
    private const int DefaultLimit = 10;
    private const int MaxLimit = 50;

    private readonly IDiscoveryService _discoveryService;

    public DiscoveryController(IDiscoveryService discoveryService)
        => _discoveryService = discoveryService;

    /// <summary>
    /// Lấy một batch hồ sơ để swipe, đã lọc theo preferences (giới tính, tuổi, khoảng cách)
    /// và sắp xếp theo khoảng cách gần nhất. Yêu cầu hồ sơ đã hoàn thiện.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<DiscoveryProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFeed([FromQuery] int limit = DefaultLimit, [FromQuery] bool includeSwiped = false)
    {
        limit = Math.Clamp(limit, 1, MaxLimit);
        var feed = await _discoveryService.GetFeedAsync(CurrentUserId, limit, includeSwiped);
        return Ok(feed);
    }
}
