using System.Security.Cryptography;
using SameMess.Application.DTOs.Safety;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class SafetyService : ISafetyService
{
    private const int OtpExpiryMinutes = 10;

    private readonly ISafetyProfileRepository _safetyRepository;
    private readonly IEmergencyContactRepository _contactRepository;
    private readonly ISafetyCheckInRepository _checkInRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOtpCodeRepository _otpCodeRepository;
    private readonly IEmailService _emailService;

    public SafetyService(
        ISafetyProfileRepository safetyRepository,
        IEmergencyContactRepository contactRepository,
        ISafetyCheckInRepository checkInRepository,
        IUserRepository userRepository,
        IOtpCodeRepository otpCodeRepository,
        IEmailService emailService)
    {
        _safetyRepository = safetyRepository;
        _contactRepository = contactRepository;
        _checkInRepository = checkInRepository;
        _userRepository = userRepository;
        _otpCodeRepository = otpCodeRepository;
        _emailService = emailService;
    }

    public async Task<SafetySettingsDto> GetSettingsAsync(Guid userId)
    {
        var profile = await GetOrCreateAsync(userId);
        return ToDto(profile);
    }

    public async Task<SafetySettingsDto> UpdateSettingsAsync(Guid userId, UpdateSafetySettingsDto dto)
    {
        var profile = await GetOrCreateAsync(userId);

        // Chỉ bật được PIN khi đã đặt PIN; tắt thì luôn được.
        if (dto.PinEnabled.HasValue)
            profile.PinEnabled = dto.PinEnabled.Value && profile.PinHash != null;
        if (dto.EmergencyAlertEnabled.HasValue) profile.EmergencyAlertEnabled = dto.EmergencyAlertEnabled.Value;
        if (dto.CheckinEnabled.HasValue) profile.CheckinEnabled = dto.CheckinEnabled.Value;
        profile.UpdatedAt = DateTime.UtcNow;

        await _safetyRepository.UpdateAsync(profile);
        await _safetyRepository.SaveChangesAsync();
        return ToDto(profile);
    }

    public async Task SetupPinAsync(Guid userId, SetupPinDto dto)
    {
        var profile = await GetOrCreateAsync(userId);
        profile.PinHash = BCrypt.Net.BCrypt.HashPassword(dto.Pin);
        profile.PinEnabled = true;
        profile.UpdatedAt = DateTime.UtcNow;

        await _safetyRepository.UpdateAsync(profile);
        await _safetyRepository.SaveChangesAsync();
    }

    public async Task<string> ForgotPinAsync(Guid userId, ForgotPinDto dto)
    {
        if (!string.Equals(dto.Channel, "email", StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("Hiện chỉ hỗ trợ gửi mã qua email.");

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        await _otpCodeRepository.InvalidateAllOtpsForEmailAsync(user.Email, OtpPurpose.PinReset);

        var code = GenerateSecureOtp();
        await _otpCodeRepository.AddAsync(new OtpCode
        {
            Id = Guid.NewGuid(),
            Email = user.Email,
            Code = code,
            Purpose = OtpPurpose.PinReset,
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow,
        });
        await _otpCodeRepository.SaveChangesAsync();

        await _emailService.SendOtpEmailAsync(user.Email, code, OtpPurpose.PinReset);
        return $"Mã xác thực đã được gửi tới {MaskEmail(user.Email)}.";
    }

    public async Task<bool> VerifyPinOtpAsync(Guid userId, VerifyPinOtpDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        var otp = await _otpCodeRepository.GetValidOtpAsync(user.Email, dto.Otp, OtpPurpose.PinReset);
        if (otp == null)
            throw new UnauthorizedException("Mã xác thực không đúng hoặc đã hết hạn.");

        otp.IsUsed = true;
        await _otpCodeRepository.UpdateAsync(otp);
        await _otpCodeRepository.SaveChangesAsync();

        // Xác thực thành công => gỡ PIN cũ để user đặt lại PIN mới.
        var profile = await GetOrCreateAsync(userId);
        profile.PinHash = null;
        profile.PinEnabled = false;
        profile.UpdatedAt = DateTime.UtcNow;
        await _safetyRepository.UpdateAsync(profile);
        await _safetyRepository.SaveChangesAsync();

        return true;
    }

    public async Task CheckinAsync(Guid userId, CheckinDto dto)
    {
        await _checkInRepository.AddAsync(new SafetyCheckIn
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow,
        });
        await _checkInRepository.SaveChangesAsync();
    }

    public async Task<EmergencyDto> GetEmergencyAsync(Guid userId)
    {
        var profile = await GetOrCreateAsync(userId);
        var contacts = await _contactRepository.GetByUserIdAsync(userId);

        return new EmergencyDto
        {
            AlertMessage = profile.AlertMessage,
            EmergencyContacts = contacts.Select(c => new EmergencyContactDto
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                Relationship = c.Relationship,
            }).ToList(),
        };
    }

    public async Task<EmergencyDto> UpsertEmergencyAsync(Guid userId, UpsertEmergencyDto dto)
    {
        var profile = await GetOrCreateAsync(userId);
        profile.AlertMessage = dto.AlertMessage;
        profile.UpdatedAt = DateTime.UtcNow;
        await _safetyRepository.UpdateAsync(profile);

        var contacts = dto.Contacts.Select(c => new EmergencyContact
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = c.Name,
            PhoneNumber = c.PhoneNumber,
            Relationship = c.Relationship,
            CreatedAt = DateTime.UtcNow,
        }).ToList();

        await _contactRepository.ReplaceForUserAsync(userId, contacts);
        await _contactRepository.SaveChangesAsync();

        return await GetEmergencyAsync(userId);
    }

    private async Task<SafetyProfile> GetOrCreateAsync(Guid userId)
    {
        var profile = await _safetyRepository.GetByUserIdAsync(userId);
        if (profile != null) return profile;

        profile = new SafetyProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
        };
        await _safetyRepository.AddAsync(profile);
        await _safetyRepository.SaveChangesAsync();
        return profile;
    }

    private static SafetySettingsDto ToDto(SafetyProfile p) => new()
    {
        PinEnabled = p.PinEnabled,
        EmergencyAlertEnabled = p.EmergencyAlertEnabled,
        CheckinEnabled = p.CheckinEnabled,
    };

    private static string GenerateSecureOtp() =>
        RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

    private static string MaskEmail(string email)
    {
        var at = email.IndexOf('@');
        if (at <= 1) return email;
        return $"{email[0]}***{email[(at - 1)..]}";
    }
}
