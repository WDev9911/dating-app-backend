namespace SameMess.Domain.Entities;

/// <summary>
/// Đăng ký nhận Web Push của một thiết bị/trình duyệt (chuẩn Web Push: endpoint + 2 khóa).
/// Frontend tạo qua service worker rồi gửi lên. Dùng để bộ gửi push thật đẩy thông báo.
/// </summary>
public class PushSubscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Endpoint { get; set; } = null!;
    public string P256dh { get; set; } = null!;  // khóa công khai client
    public string Auth { get; set; } = null!;     // khóa xác thực
    public DateTime CreatedAt { get; set; }
}
