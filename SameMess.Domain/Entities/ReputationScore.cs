namespace SameMess.Domain.Entities;

/// <summary>
/// Điểm uy tín hiện tại của một user (bản chụp được tính lại từ log ReputationEvent
/// mỗi khi có sự kiện mới — cache để Discovery xếp hạng nhanh, không phải duyệt log).
/// </summary>
public class ReputationScore
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Score { get; set; }          // 0..100
    public string Tier { get; set; } = null!; // ReputationTier
    public DateTime UpdatedAt { get; set; }
}
