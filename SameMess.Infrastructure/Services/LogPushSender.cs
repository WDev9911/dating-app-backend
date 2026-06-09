using Microsoft.Extensions.Logging;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Infrastructure.Services;

/// <summary>
/// Bộ gửi push mặc định: KHÔNG đẩy thật, chỉ ghi log cho mỗi đăng ký của user
/// (đủ để dev/test toàn luồng). Thay bằng WebPushSender/FcmPushSender khi có VAPID/FCM + frontend.
/// </summary>
public class LogPushSender : IPushSender
{
    private readonly IPushSubscriptionRepository _subscriptionRepository;
    private readonly ILogger<LogPushSender> _logger;

    public LogPushSender(IPushSubscriptionRepository subscriptionRepository, ILogger<LogPushSender> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
    }

    public async Task SendAsync(Guid userId, string title, string body, string? data = null)
    {
        var subs = await _subscriptionRepository.GetByUserAsync(userId);
        _logger.LogInformation(
            "[PUSH] user={UserId} devices={Count} title=\"{Title}\" body=\"{Body}\"",
            userId, subs.Count, title, body);
    }
}
