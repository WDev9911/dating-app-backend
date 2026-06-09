namespace SameMess.Domain.Entities;

/// <summary>
/// Một cặp match. Luôn chuẩn hóa UserAId &lt; UserBId để mỗi cặp chỉ có một bản ghi
/// (tránh trùng A-B vs B-A). Unique index trên (UserAId, UserBId) bảo vệ ở tầng DB.
/// </summary>
public class Match
{
    public Guid Id { get; set; }
    public Guid UserAId { get; set; }
    public Guid UserBId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
