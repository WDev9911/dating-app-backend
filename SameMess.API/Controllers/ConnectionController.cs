using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Connection;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/connection")]
public class ConnectionController : ApiControllerBase
{
    private readonly IConnectionService _connectionService;

    public ConnectionController(IConnectionService connectionService)
        => _connectionService = connectionService;

    /// <summary>Nhắc nhở giữ kết nối (match chưa trò chuyện / hội thoại lặng).</summary>
    [HttpGet("reminders")]
    [ProducesResponseType(typeof(List<ReminderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReminders()
        => Ok(await _connectionService.GetRemindersAsync(CurrentUserId));

    /// <summary>Gợi ý (nudges) cho một hội thoại.</summary>
    [HttpGet("nudges/{conversationId:guid}")]
    [ProducesResponseType(typeof(List<NudgeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNudges(Guid conversationId)
        => Ok(await _connectionService.GetNudgesAsync(CurrentUserId, conversationId));

    /// <summary>Bỏ qua một gợi ý (để không hiện lại).</summary>
    [HttpPost("nudges/{conversationId:guid}/dismiss")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DismissNudge(Guid conversationId, [FromBody] DismissNudgeDto dto)
    {
        await _connectionService.DismissNudgeAsync(CurrentUserId, conversationId, dto.NudgeId);
        return Ok(new { dismissed = true });
    }

    /// <summary>Đề xuất gặp mặt trong một hội thoại.</summary>
    [HttpPost("meetup/{conversationId:guid}/propose")]
    [ProducesResponseType(typeof(MeetupResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ProposeMeetup(Guid conversationId, [FromBody] ProposeMeetupDto dto)
        => Ok(await _connectionService.ProposeMeetupAsync(CurrentUserId, conversationId, dto));
}
