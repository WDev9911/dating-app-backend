using SameMess.Application.DTOs.Preference;

namespace SameMess.Application.Interfaces.Services;

public interface IPreferenceService
{
    Task<PreferenceDto> GetMyPreferenceAsync(Guid userId);
    Task<PreferenceDto> UpdatePreferenceAsync(Guid userId, UpdatePreferenceDto dto);
}
