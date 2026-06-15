using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
using SameMess.Application.DTOs.Preference;
using SameMess.Application.DTOs.Settings;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/settings")]
public class SettingsController : ApiControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly IPreferenceService _preferenceService;
    private readonly IInterestsService _interestsService;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
    private readonly IValidator<UpdatePreferenceDto> _updatePreferenceValidator;

    public SettingsController(
        ISettingsService settingsService,
        IPreferenceService preferenceService,
        IInterestsService interestsService,
        IValidator<ChangePasswordDto> changePasswordValidator,
        IValidator<UpdatePreferenceDto> updatePreferenceValidator)
    {
        _settingsService = settingsService;
        _preferenceService = preferenceService;
        _interestsService = interestsService;
        _changePasswordValidator = changePasswordValidator;
        _updatePreferenceValidator = updatePreferenceValidator;
    }

    /// <summary>Đổi mật khẩu. Sau khi đổi, mọi phiên đăng nhập bị thu hồi (phải đăng nhập lại).</summary>
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var validation = await _changePasswordValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        await _settingsService.ChangePasswordAsync(CurrentUserId, dto);
        return NoContent();
    }

    /// <summary>Lấy cài đặt bảo mật (trạng thái xác minh, 2FA, cảnh báo đăng nhập).</summary>
    [HttpGet("security")]
    [ProducesResponseType(typeof(SecuritySettingsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSecurity()
        => Ok(await _settingsService.GetSecurityAsync(CurrentUserId));

    /// <summary>Cập nhật cài đặt bảo mật (2FA, cảnh báo đăng nhập).</summary>
    [HttpPut("security")]
    [ProducesResponseType(typeof(SecuritySettingsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSecurity([FromBody] UpdateSecuritySettingsDto dto)
        => Ok(await _settingsService.UpdateSecurityAsync(CurrentUserId, dto));

    /// <summary>Danh sách thiết bị/phiên đăng nhập đang hoạt động (từ refresh token).</summary>
    [HttpGet("devices")]
    [ProducesResponseType(typeof(List<DeviceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDevices()
        => Ok(await _settingsService.GetDevicesAsync(CurrentUserId));

    /// <summary>Lấy cài đặt khám phá (tiêu chí lọc: giới tính, tuổi, khoảng cách).</summary>
    [HttpGet("discovery")]
    [ProducesResponseType(typeof(PreferenceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDiscovery()
        => Ok(await _preferenceService.GetMyPreferenceAsync(CurrentUserId));

    /// <summary>Cập nhật cài đặt khám phá (dùng chung với /api/preferences).</summary>
    [HttpPut("discovery")]
    [ProducesResponseType(typeof(PreferenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDiscovery([FromBody] UpdatePreferenceDto dto)
    {
        var validation = await _updatePreferenceValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        return Ok(await _preferenceService.UpdatePreferenceAsync(CurrentUserId, dto));
    }

    /// <summary>Lấy danh sách sở thích đã chọn của tôi.</summary>
    [HttpGet("interests")]
    [ProducesResponseType(typeof(List<InterestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInterests()
        => Ok(await _interestsService.GetMyInterestsAsync(CurrentUserId));

    /// <summary>Cập nhật sở thích của tôi (gửi danh sách interestId, tối đa 10).</summary>
    [HttpPut("interests")]
    [ProducesResponseType(typeof(List<InterestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateInterests([FromBody] UpdateInterestsDto dto)
        => Ok(await _interestsService.UpdateMyInterestsAsync(CurrentUserId, dto));
}
