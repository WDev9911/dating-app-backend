namespace SameMess.Application.DTOs.Reputation;

/// <summary>Chi tiết uy tín cho CHÍNH CHỦ (người khác chỉ thấy Tier qua Discovery, không thấy số).</summary>
public class ReputationDto
{
    public int Score { get; set; }              // 0..100 — chỉ chính chủ thấy số
    public string Tier { get; set; } = null!;
    public string TierLabel { get; set; } = null!;
    public bool FaceVerified { get; set; }
    public bool IsCapped { get; set; }          // đang bị trần 65 vì chưa xác minh mặt
    public int Cap { get; set; }                // mức trần hiện hành
    public List<ReputationEventDto> RecentEvents { get; set; } = new();
    public List<string> HowToImprove { get; set; } = new();
}
