namespace SameMess.Application.DTOs.Verification;

/// <summary>Trạng thái xác minh khuôn mặt (cho chính chủ).</summary>
public class VerificationStatusDto
{
    public string Status { get; set; } = null!;   // VerificationStatus
    public bool IsPhotoVerified { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>Một hồ sơ đang chờ admin duyệt khuôn mặt.</summary>
public class PendingVerificationDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? SelfieUrl { get; set; }
    public string? ProfilePhotoUrl { get; set; }
}

public class ReviewVerificationDto
{
    public bool Approve { get; set; }
}
