using SameMess.Application.DTOs.Auth;

namespace SameMess.Application.Interfaces.Services;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<AuthTokenResult> VerifyEmailAsync(VerifyOtpRequestDto dto);
    Task<AuthTokenResult> LoginAsync(LoginRequestDto dto, string? ipAddress, string? userAgent);
    Task<AuthTokenResult> RefreshTokenAsync(string plainRefreshToken, string? ipAddress, string? userAgent);
    Task RevokeRefreshTokenAsync(string plainRefreshToken);
    Task<UserInfoDto> GetCurrentUserAsync(Guid userId);
}
