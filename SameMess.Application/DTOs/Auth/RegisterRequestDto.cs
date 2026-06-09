namespace SameMess.Application.DTOs.Auth;

public class RegisterRequestDto
{
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}
