namespace SameMess.Application.DTOs.Matching;

public class SwipeRequestDto
{
    public Guid TargetUserId { get; set; }
    public string Action { get; set; } = null!; // Like | Pass | SuperLike
}
