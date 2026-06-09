namespace SameMess.Application.Verification;

/// <summary>
/// Ngưỡng quyết định xác minh khuôn mặt (distance dlib: càng nhỏ càng giống).
/// Rất giống → tự duyệt; khác hẳn → tự từ chối; ở giữa → chuyển admin duyệt tay.
/// </summary>
public static class FaceVerificationConfig
{
    public const double AutoApproveBelow = 0.40; // distance < 0.40 → tự Approved
    public const double AutoRejectAbove = 0.60;  // distance > 0.60 → tự Rejected
    // 0.40..0.60 → Pending (admin duyệt)
}
