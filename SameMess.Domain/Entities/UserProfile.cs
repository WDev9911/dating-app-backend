namespace SameMess.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Bio { get; set; }
    public int? Height { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? LocationUpdatedAt { get; set; }
    public string? AvatarUrl { get; set; }
    public string? DatingGoal { get; set; }
    public bool IsProfileCompleted { get; set; }
    public DateTime? BoostedUntil { get; set; }

    // Xác minh khuôn mặt
    public bool IsPhotoVerified { get; set; }
    public string VerificationStatus { get; set; } = Enums.VerificationStatus.None;
    public string? VerificationSelfieUrl { get; set; } // chỉ giữ khi đang Pending (admin xem); xóa sau khi quyết định

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
