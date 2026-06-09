namespace SameMess.Application.DTOs.Auth;

public class AuthTokenResult
{
    public AuthResponseDto Response { get; init; } = null!;
    public string PlainRefreshToken { get; init; } = null!;
}
