using SameMess.Application.DTOs.Profile;

namespace SameMess.Application.DTOs.Matching;

/// <summary>
/// Hồ sơ một người đã Like tôi (mà tôi chưa swipe lại).
/// Đây là tính năng có thể khóa sau paywall (premium) ở giai đoạn thương mại hóa.
/// </summary>
public class LikedMeProfileDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public string? Bio { get; set; }
    public bool IsSuperLike { get; set; }

    /// <summary>True nếu ảnh bị khóa do chưa nâng cấp (Free) — backend KHÔNG trả URL ảnh gốc.</summary>
    public bool PhotosLocked { get; set; }
    public List<PhotoDto> Photos { get; set; } = new();
}
