using SameMess.Application.DTOs.Notifications;

namespace SameMess.Application.Interfaces.Services;

public interface INotificationService
{
    /// <summary>Tạo thông báo cho user (lưu feed) + đẩy push (fail-safe). Gọi từ các service sẵn có.</summary>
    Task NotifyAsync(Guid userId, string type, string title, string body, string? data = null);

    Task<NotificationFeedDto> GetFeedAsync(Guid userId, int limit);
    Task MarkAllReadAsync(Guid userId);

    Task SubscribeAsync(Guid userId, PushSubscriptionDto dto);
    Task UnsubscribeAsync(Guid userId, string endpoint);
}
