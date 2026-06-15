using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AdminSettingsService : IAdminSettingsService
{
    private readonly IAppSettingRepository _settingRepository;
    private readonly IFeatureFlagRepository _flagRepository;
    private readonly IAuditService _auditService;

    public AdminSettingsService(
        IAppSettingRepository settingRepository,
        IFeatureFlagRepository flagRepository,
        IAuditService auditService)
    {
        _settingRepository = settingRepository;
        _flagRepository = flagRepository;
        _auditService = auditService;
    }

    public async Task<AdminSettingsDto> GetAsync()
    {
        var settings = await _settingRepository.GetAllAsync();
        var flags = await _flagRepository.GetAllAsync();
        return new AdminSettingsDto
        {
            Settings = settings.ToDictionary(s => s.Key, s => s.Value),
            FeatureFlags = flags.ToDictionary(f => f.Key, f => f.IsEnabled),
        };
    }

    public async Task<AdminSettingsDto> UpdateSettingsAsync(Guid adminId, UpdateSettingsDto dto)
    {
        foreach (var kv in dto.Settings)
            await _settingRepository.UpsertAsync(kv.Key, kv.Value);
        await _settingRepository.SaveChangesAsync();

        await _auditService.LogAsync(adminId, "settings.update", "AppSetting", null,
            string.Join(",", dto.Settings.Keys));

        return await GetAsync();
    }

    public async Task SetFeatureFlagAsync(Guid adminId, string key, bool value)
    {
        await _flagRepository.UpsertAsync(key, value);
        await _flagRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "feature-flag.set", "FeatureFlag", null, $"{key}={value}");
    }
}
