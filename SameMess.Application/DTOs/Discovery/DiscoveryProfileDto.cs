using SameMess.Application.DTOs.Profile;

namespace SameMess.Application.DTOs.Discovery;

/// <summary>
/// Hồ sơ một ứng viên hiển thị trong feed. KHÔNG chứa tọa độ chính xác của người khác —
/// chỉ trả khoảng cách đã làm tròn (km) để bảo vệ quyền riêng tư.
/// </summary>
public class DiscoveryProfileDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public int Age { get; set; }
    public string? Gender { get; set; }
    public string? Bio { get; set; }
    public int? Height { get; set; }
    public string? Location { get; set; }
    public string? DatingGoal { get; set; }
    public int DistanceKm { get; set; }
    public bool IsBoosted { get; set; }
    public bool IsPhotoVerified { get; set; }
    public string ReputationTier { get; set; } = Domain.Enums.ReputationTier.Normal; // chỉ mức/badge, không lộ số điểm
    public List<PhotoDto> Photos { get; set; } = new();
}
