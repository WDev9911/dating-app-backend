namespace SameMess.Application.DTOs.Auth;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public bool IsEmailVerified { get; set; }
    public string Role { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string? AvatarFrame { get; set; }
}
