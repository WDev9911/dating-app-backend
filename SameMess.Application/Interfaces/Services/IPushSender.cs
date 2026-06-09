namespace SameMess.Application.Interfaces.Services;

/// <summary>
/// Trừu tượng kênh đẩy push tới thiết bị của user. Impl mặc định ghi log (LogPushSender);
/// thay bằng WebPushSender (VAPID) hoặc FcmPushSender chỉ cần đổi DI — phần còn lại không đổi.
/// </summary>
public interface IPushSender
{
    Task SendAsync(Guid userId, string title, string body, string? data = null);
}
