namespace SameMess.Domain.Entities;

/// <summary>
/// Log từng thay đổi điểm uy tín (minh bạch + audit). Điểm được TÍNH LẠI từ log:
/// điểm = khởi_điểm + Σ(Delta × hệ_số_phai theo Severity/tuổi).
/// </summary>
public class ReputationEvent
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = null!;     // ReputationEventType
    public int Delta { get; set; }                 // +/- điểm gốc của sự kiện
    public string Severity { get; set; } = null!;  // ReputationSeverity
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
