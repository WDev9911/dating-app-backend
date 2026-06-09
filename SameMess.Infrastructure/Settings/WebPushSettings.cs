namespace SameMess.Infrastructure.Settings;

/// <summary>
/// Khóa VAPID cho Web Push (sinh bằng web-push generate-vapid-keys). Để trống lúc dev;
/// bộ gửi thật (WebPushSender) sẽ cần khi tích hợp với service worker phía frontend.
/// </summary>
public class WebPushSettings
{
    public string PublicKey { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;
    public string Subject { get; set; } = "mailto:admin@samemess.app";
}
