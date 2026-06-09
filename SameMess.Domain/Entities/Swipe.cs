namespace SameMess.Domain.Entities;

public class Swipe
{
    public Guid Id { get; set; }
    public Guid SwiperId { get; set; }
    public Guid TargetUserId { get; set; }
    public string Action { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
