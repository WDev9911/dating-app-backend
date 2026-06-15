namespace SameMess.Domain.Entities;

/// <summary>Một lần user hoàn thành nhiệm vụ hằng ngày (mỗi quest/ngày tối đa 1 lần).</summary>
public class DailyQuestCompletion
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string QuestCode { get; set; } = null!;
    public string PeriodKey { get; set; } = null!;  // yyyyMMdd (UTC)
    public int XpAwarded { get; set; }
    public DateTime CompletedAt { get; set; }
}
