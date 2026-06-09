using SameMess.Domain.Enums;

namespace SameMess.Domain.Entities;

public class UserPreference
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string InterestedInGender { get; set; } = GenderPreference.Everyone;
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 99;
    public int MaxDistanceKm { get; set; } = 50;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
