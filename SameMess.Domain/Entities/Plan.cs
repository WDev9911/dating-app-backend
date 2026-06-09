namespace SameMess.Domain.Entities;

/// <summary>
/// Định nghĩa gói thuê bao (để DB để chỉnh giá không cần build lại). Quyền lợi (entitlements)
/// suy ra từ PlanCode trong Application; bảng này giữ giá + thời hạn để hiển thị/tính tiền.
/// </summary>
public class Plan
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;   // PlanCode
    public string Name { get; set; } = null!;
    public int PriceVnd { get; set; }            // giá 1 kỳ (VNĐ)
    public int DurationDays { get; set; }        // số ngày hiệu lực mỗi lần mua
    public bool IsActive { get; set; } = true;
}
