namespace SameMess.Domain.Entities;

/// <summary>Nhật ký hành động của admin (audit trail).</summary>
public class AuditLog
{
    public Guid Id { get; set; }
    public Guid AdminId { get; set; }
    public string Action { get; set; } = null!;
    public string? TargetType { get; set; }
    public Guid? TargetId { get; set; }
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; }
}
