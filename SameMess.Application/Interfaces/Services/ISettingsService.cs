using SameMess.Application.DTOs.Settings;

namespace SameMess.Application.Interfaces.Services;

public interface ISettingsService
{
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    Task<SecuritySettingsDto> GetSecurityAsync(Guid userId);
    Task<SecuritySettingsDto> UpdateSecurityAsync(Guid userId, UpdateSecuritySettingsDto dto);
    Task<List<DeviceDto>> GetDevicesAsync(Guid userId);
}
