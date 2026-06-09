using SameMess.Application.DTOs.Notifications;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IPushSubscriptionRepository _subscriptionRepository;
    private readonly IPushSender _pushSender;

    public NotificationService(
        INotificationRepository notificationRepository,
        IPushSubscriptionRepository subscriptionRepository,
        IPushSender pushSender)
    {
        _notificationRepository = notificationRepository;
        _subscriptionRepository = subscriptionRepository;
        _pushSender = pushSender;
    }

    public async Task NotifyAsync(Guid userId, string type, string title, string body, string? data = null)
    {
        await _notificationRepository.AddAsync(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Title = title,
            Body = body,
            Data = data,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
        });
        await _notificationRepository.SaveChangesAsync();

        // Đẩy push (fail-safe — lỗi push không được làm hỏng việc lưu thông báo)
        try { await _pushSender.SendAsync(userId, title, body, data); }
        catch { /* bỏ qua lỗi kênh push */ }
    }

    public async Task<NotificationFeedDto> GetFeedAsync(Guid userId, int limit)
    {
        var items = await _notificationRepository.GetForUserAsync(userId, limit);
        var unread = await _notificationRepository.CountUnreadAsync(userId);

        return new NotificationFeedDto
        {
            UnreadCount = unread,
            Items = items.Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Title = n.Title,
                Body = n.Body,
                Data = n.Data,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
            }).ToList(),
        };
    }

    public async Task MarkAllReadAsync(Guid userId) =>
        await _notificationRepository.MarkAllReadAsync(userId);

    public async Task SubscribeAsync(Guid userId, PushSubscriptionDto dto)
    {
        // Idempotent theo (user, endpoint)
        var existing = await _subscriptionRepository.GetByEndpointAsync(userId, dto.Endpoint);
        if (existing is not null)
        {
            existing.P256dh = dto.P256dh;
            existing.Auth = dto.Auth;
        }
        else
        {
            await _subscriptionRepository.AddAsync(new Domain.Entities.PushSubscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Endpoint = dto.Endpoint,
                P256dh = dto.P256dh,
                Auth = dto.Auth,
                CreatedAt = DateTime.UtcNow,
            });
        }
        await _subscriptionRepository.SaveChangesAsync();
    }

    public async Task UnsubscribeAsync(Guid userId, string endpoint)
    {
        var sub = await _subscriptionRepository.GetByEndpointAsync(userId, endpoint);
        if (sub is null) return;
        await _subscriptionRepository.DeleteAsync(sub);
        await _subscriptionRepository.SaveChangesAsync();
    }
}
