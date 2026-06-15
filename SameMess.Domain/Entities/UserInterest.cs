namespace SameMess.Domain.Entities;

/// <summary>Liên kết user ↔ sở thích (n-n).</summary>
public class UserInterest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid InterestId { get; set; }

    public User User { get; set; } = null!;
    public Interest Interest { get; set; } = null!;
}
