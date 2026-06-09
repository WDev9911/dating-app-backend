namespace SameMess.Application.DTOs.Safety;

public class BlockedUserDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public DateTime BlockedAt { get; set; }
}
