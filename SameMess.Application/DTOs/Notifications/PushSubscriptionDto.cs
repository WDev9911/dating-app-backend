namespace SameMess.Application.DTOs.Notifications;

/// <summary>Dữ liệu đăng ký Web Push do frontend (service worker) tạo ra và gửi lên.</summary>
public class PushSubscriptionDto
{
    public string Endpoint { get; set; } = null!;
    public string P256dh { get; set; } = null!;
    public string Auth { get; set; } = null!;
}
