using SameMess.Application.DTOs.Admin;

namespace SameMess.Application.Interfaces.Services;

public interface IAdminEventService
{
    Task<List<AdminEventDto>> ListAsync();
    Task<AdminEventDto> GetAsync(Guid id);
    Task<AdminEventDto> CreateAsync(Guid adminId, EventPayloadDto dto);
    Task<AdminEventDto> UpdateAsync(Guid adminId, Guid id, EventPayloadDto dto);
    Task DeleteAsync(Guid adminId, Guid id);
    Task<AdminEventDto> SetPublishedAsync(Guid adminId, Guid id, bool published);
    Task<List<EventRegistrationDto>> GetRegistrationsAsync(Guid id);
}

public interface IAdminPlanService
{
    Task<List<AdminPlanDto>> ListAsync();
    Task<AdminPlanDto> CreateAsync(Guid adminId, PlanPayloadDto dto);
    Task<AdminPlanDto> UpdateAsync(Guid adminId, Guid id, PlanPayloadDto dto);
    Task DeleteAsync(Guid adminId, Guid id);
    Task<List<SubscriberDto>> GetSubscribersAsync(string? planCode, string? status);
}

public interface IAdminInterestService
{
    Task<List<AdminInterestDto>> ListAsync();
    Task<AdminInterestDto> CreateAsync(Guid adminId, InterestPayloadDto dto);
    Task<AdminInterestDto> UpdateAsync(Guid adminId, Guid id, InterestPayloadDto dto);
    Task DeleteAsync(Guid adminId, Guid id);
    Task<string> CreateGroupAsync(Guid adminId, string name);
}

public interface IAdminSettingsService
{
    Task<AdminSettingsDto> GetAsync();
    Task<AdminSettingsDto> UpdateSettingsAsync(Guid adminId, UpdateSettingsDto dto);
    Task SetFeatureFlagAsync(Guid adminId, string key, bool value);
}
