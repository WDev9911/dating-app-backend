using SameMess.Application.DTOs.Settings;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public SettingsService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        // Đổi mật khẩu => thu hồi mọi phiên đăng nhập (buộc đăng nhập lại trên mọi thiết bị).
        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
        await _refreshTokenRepository.SaveChangesAsync();
    }

    public async Task<SecuritySettingsDto> GetSecurityAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        return ToDto(user.Email, user.IsEmailVerified, user.IsPhoneVerified,
            user.TwoFactorEnabled, user.LoginAlertsEnabled);
    }

    public async Task<SecuritySettingsDto> UpdateSecurityAsync(Guid userId, UpdateSecuritySettingsDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        if (dto.TwoFactorEnabled.HasValue) user.TwoFactorEnabled = dto.TwoFactorEnabled.Value;
        if (dto.LoginAlertsEnabled.HasValue) user.LoginAlertsEnabled = dto.LoginAlertsEnabled.Value;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return ToDto(user.Email, user.IsEmailVerified, user.IsPhoneVerified,
            user.TwoFactorEnabled, user.LoginAlertsEnabled);
    }

    public async Task<List<DeviceDto>> GetDevicesAsync(Guid userId)
    {
        var tokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(userId);
        return tokens
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new DeviceDto
            {
                Id = t.Id,
                IpAddress = t.CreatedByIp,
                UserAgent = t.UserAgent,
                CreatedAt = t.CreatedAt,
                ExpiresAt = t.ExpiresAt,
            })
            .ToList();
    }

    private static SecuritySettingsDto ToDto(string email, bool emailVerified, bool phoneVerified,
        bool twoFactor, bool loginAlerts) => new()
    {
        Email = email,
        IsEmailVerified = emailVerified,
        IsPhoneVerified = phoneVerified,
        TwoFactorEnabled = twoFactor,
        LoginAlertsEnabled = loginAlerts,
    };
}
