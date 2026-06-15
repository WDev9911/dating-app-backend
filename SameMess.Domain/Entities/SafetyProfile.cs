namespace SameMess.Domain.Entities;

/// <summary>Cài đặt an toàn của một user (1-1): PIN khóa app, cảnh báo khẩn cấp, check-in.</summary>
public class SafetyProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public bool PinEnabled { get; set; }
    public string? PinHash { get; set; }            // BCrypt của mã PIN; null nếu chưa đặt

    public bool EmergencyAlertEnabled { get; set; }
    public bool CheckinEnabled { get; set; }
    public string? AlertMessage { get; set; }       // tin nhắn gửi liên hệ khẩn cấp khi báo động

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
