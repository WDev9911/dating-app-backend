namespace SameMess.Domain.Entities;

/// <summary>Lịch sử check-in an toàn của user ("safe" | "help").</summary>
public class SafetyCheckIn
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; } = null!;   // "safe" | "help"
    public DateTime CreatedAt { get; set; }
}
