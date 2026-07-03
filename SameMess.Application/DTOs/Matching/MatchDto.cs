namespace SameMess.Application.DTOs.Matching;

/// <summary>Một match trong danh sách của tôi — chứa thông tin cơ bản của người đối diện.</summary>
public class MatchDto
{
    public Guid MatchId { get; set; }
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public int? Age { get; set; }
    public DateTime MatchedAt { get; set; }
    public bool IsAdmin { get; set; }
    public string? AvatarFrame { get; set; }
}
