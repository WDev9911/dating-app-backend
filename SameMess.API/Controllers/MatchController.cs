using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Matching;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/matches")]
public class MatchController : ApiControllerBase
{
    private readonly IMatchService _matchService;

    public MatchController(IMatchService matchService) => _matchService = matchService;

    /// <summary>Danh sách match đang hoạt động của tôi (kèm info người đối diện).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<MatchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyMatches()
        => Ok(await _matchService.GetMyMatchesAsync(CurrentUserId));

    /// <summary>Hủy match (unmatch).</summary>
    [HttpDelete("{matchId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unmatch(Guid matchId)
    {
        await _matchService.UnmatchAsync(CurrentUserId, matchId);
        return NoContent();
    }
}
