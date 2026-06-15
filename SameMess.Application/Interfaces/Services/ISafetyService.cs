using SameMess.Application.DTOs.Safety;

namespace SameMess.Application.Interfaces.Services;

public interface ISafetyService
{
    Task<SafetySettingsDto> GetSettingsAsync(Guid userId);
    Task<SafetySettingsDto> UpdateSettingsAsync(Guid userId, UpdateSafetySettingsDto dto);
    Task SetupPinAsync(Guid userId, SetupPinDto dto);
    Task<string> ForgotPinAsync(Guid userId, ForgotPinDto dto);
    Task<bool> VerifyPinOtpAsync(Guid userId, VerifyPinOtpDto dto);
    Task CheckinAsync(Guid userId, CheckinDto dto);
    Task<EmergencyDto> GetEmergencyAsync(Guid userId);
    Task<EmergencyDto> UpsertEmergencyAsync(Guid userId, UpsertEmergencyDto dto);
}
