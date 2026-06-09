namespace SameMess.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;
    public UserInfoDto User { get; set; } = null!;
}
