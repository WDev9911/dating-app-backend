using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Reputation;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/reputation")]
public class ReputationController : ApiControllerBase
{
    private readonly IReputationService _reputationService;

    public ReputationController(IReputationService reputationService)
        => _reputationService = reputationService;

    /// <summary>Điểm uy tín của chính tôi (kèm số điểm, mức, lịch sử sự kiện, cách cải thiện).</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ReputationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyReputation()
        => Ok(await _reputationService.GetMyReputationAsync(CurrentUserId));
}
