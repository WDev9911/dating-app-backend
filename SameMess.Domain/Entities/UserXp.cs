namespace SameMess.Domain.Entities;

/// <summary>Tổng điểm XP engagement của user (từ nhiệm vụ hằng ngày). Tách khỏi điểm uy tín & cây tình yêu.</summary>
public class UserXp
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int TotalXp { get; set; }
    public DateTime UpdatedAt { get; set; }
}
