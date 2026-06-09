namespace SameMess.Domain.Enums;

/// <summary>Trạng thái xác minh khuôn mặt của hồ sơ.</summary>
public static class VerificationStatus
{
    public const string None = "None";         // chưa gửi
    public const string Pending = "Pending";   // chờ admin duyệt (so khớp không chắc)
    public const string Approved = "Approved"; // đã xác minh ✓
    public const string Rejected = "Rejected"; // bị từ chối
}
