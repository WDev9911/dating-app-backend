using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Events;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/events")]
public class EventsController : ApiControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService) => _eventService = eventService;

    /// <summary>Danh sách sự kiện đã công bố (kèm số lượng đăng ký + tôi đã đăng ký chưa).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEvents()
        => Ok(await _eventService.GetEventsAsync(CurrentUserId));

    /// <summary>Lịch sử sự kiện tôi đã đăng ký.</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(List<EventHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory()
        => Ok(await _eventService.GetHistoryAsync(CurrentUserId));

    /// <summary>Phần thưởng của một sự kiện (XP + badge).</summary>
    [HttpGet("reward")]
    [ProducesResponseType(typeof(EventRewardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReward([FromQuery] Guid eventId)
        => Ok(await _eventService.GetRewardAsync(eventId));

    /// <summary>Chi tiết một sự kiện.</summary>
    [HttpGet("{eventId:guid}")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEvent(Guid eventId)
        => Ok(await _eventService.GetEventAsync(CurrentUserId, eventId));

    /// <summary>Đăng ký tham gia sự kiện.</summary>
    [HttpPost("{eventId:guid}/register")]
    [ProducesResponseType(typeof(RegisterResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Register(Guid eventId)
        => Ok(await _eventService.RegisterAsync(CurrentUserId, eventId));
}
