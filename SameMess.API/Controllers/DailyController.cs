using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Daily;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/daily")]
public class DailyController : ApiControllerBase
{
    private readonly IDailyService _dailyService;

    public DailyController(IDailyService dailyService) => _dailyService = dailyService;

    /// <summary>Nhiệm vụ hằng ngày + XP (đã hoàn thành cái nào hôm nay).</summary>
    [HttpGet("connection")]
    [ProducesResponseType(typeof(DailyConnectionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConnection()
        => Ok(await _dailyService.GetConnectionAsync(CurrentUserId));

    /// <summary>Báo hoàn thành nhiệm vụ (gửi questIds) → cộng XP (idempotent theo ngày).</summary>
    [HttpPost("complete")]
    [ProducesResponseType(typeof(CompleteDailyResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Complete([FromBody] CompleteDailyDto dto)
        => Ok(await _dailyService.CompleteAsync(CurrentUserId, dto));
}
