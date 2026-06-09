namespace SameMess.Domain.Entities;

/// <summary>
/// Thuê bao hiện tại của một user (mỗi user tối đa một bản ghi — unique UserId).
/// Hết hạn (ExpiresAt &lt; now) coi như về Free. Mua thêm sẽ gia hạn từ mốc còn lại.
/// </summary>
public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PlanCode { get; set; } = null!;
    public DateTime StartAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
