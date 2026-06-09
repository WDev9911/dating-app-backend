namespace SameMess.Domain.Entities;

/// <summary>
/// Tiến độ một nhiệm vụ của một user trong một kỳ (PeriodKey). Reset bằng PeriodKey
/// (yyyy-MM-dd / yyyy-Www / "ALL") nên không cần cron — sang kỳ mới là dòng mới.
/// </summary>
public class UserTaskProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TaskCode { get; set; } = null!;
    public string PeriodKey { get; set; } = null!;
    public int Progress { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
}
