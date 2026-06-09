using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/ai")]
public class AiController : ApiControllerBase
{
    private readonly IIcebreakerService _icebreakerService;

    public AiController(IIcebreakerService icebreakerService)
        => _icebreakerService = icebreakerService;

    /// <summary>Gợi ý câu mở lời cho một match (dựa trên hồ sơ 2 người, sinh bởi AI).</summary>
    [HttpPost("icebreakers/{matchId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIcebreakers(Guid matchId)
    {
        var suggestions = await _icebreakerService.SuggestForMatchAsync(CurrentUserId, matchId);
        return Ok(new { suggestions });
    }
}
