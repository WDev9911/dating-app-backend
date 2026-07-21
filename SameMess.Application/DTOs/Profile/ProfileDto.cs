using SameMess.Application.DTOs.Preference;
using SameMess.Application.DTOs.Review;

namespace SameMess.Application.DTOs.Profile;

public class ProfileDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string? Bio { get; set; }
    public int? Height { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? DatingGoal { get; set; }
    public string? AvatarUrl { get; set; }
    public string? AvatarFrame { get; set; }
    public bool IsProfileCompleted { get; set; }
    public bool IsPhotoVerified { get; set; }
    public string VerificationStatus { get; set; } = Domain.Enums.VerificationStatus.None;
    public bool IsAdmin { get; set; }
    public List<PhotoDto> Photos { get; set; } = new();
    public PreferenceDto? Preference { get; set; }

    // Đánh giá sau buổi hẹn (điểm TB hiện cho mọi người; danh sách chỉ mở cho Gold)
    public double RatingAvg { get; set; }
    public int RatingCount { get; set; }
    public bool ReviewsLocked { get; set; }
    public List<DateReviewDto> Reviews { get; set; } = new();
}
