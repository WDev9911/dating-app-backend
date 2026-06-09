using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Notifications;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/notifications")]
public class NotificationsController : ApiControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IConfiguration _configuration;

    public NotificationsController(INotificationService notificationService, IConfiguration configuration)
    {
        _notificationService = notificationService;
        _configuration = configuration;
    }

    /// <summary>Feed thông báo của tôi + số chưa đọc.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(NotificationFeedDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeed([FromQuery] int limit = 30)
        => Ok(await _notificationService.GetFeedAsync(CurrentUserId, Math.Clamp(limit, 1, 100)));

    /// <summary>Đánh dấu đã đọc tất cả thông báo.</summary>
    [HttpPost("read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllRead()
    {
        await _notificationService.MarkAllReadAsync(CurrentUserId);
        return NoContent();
    }

    /// <summary>Đăng ký nhận Web Push (frontend tạo qua service worker).</summary>
    [HttpPost("subscribe")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Subscribe([FromBody] PushSubscriptionDto dto)
    {
        await _notificationService.SubscribeAsync(CurrentUserId, dto);
        return NoContent();
    }

    /// <summary>Hủy đăng ký Web Push theo endpoint.</summary>
    [HttpPost("unsubscribe")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Unsubscribe([FromBody] PushSubscriptionDto dto)
    {
        await _notificationService.UnsubscribeAsync(CurrentUserId, dto.Endpoint);
        return NoContent();
    }

    /// <summary>Khóa công khai VAPID để frontend đăng ký Web Push.</summary>
    [HttpGet("vapid-public-key")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetVapidPublicKey()
        => Ok(new { publicKey = _configuration["WebPush:PublicKey"] ?? string.Empty });
}
