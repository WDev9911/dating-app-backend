namespace SameMess.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = null!;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public string Role { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public UserProfile? Profile { get; set; }
    public UserPreference? Preference { get; set; }
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
