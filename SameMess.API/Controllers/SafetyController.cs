using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
using SameMess.Application.DTOs.Safety;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/safety")]
public class SafetyController : ApiControllerBase
{
    private readonly ISafetyService _safetyService;
    private readonly IValidator<SetupPinDto> _setupPinValidator;
    private readonly IValidator<CheckinDto> _checkinValidator;

    public SafetyController(
        ISafetyService safetyService,
        IValidator<SetupPinDto> setupPinValidator,
        IValidator<CheckinDto> checkinValidator)
    {
        _safetyService = safetyService;
        _setupPinValidator = setupPinValidator;
        _checkinValidator = checkinValidator;
    }

    /// <summary>Lấy cài đặt an toàn (PIN/cảnh báo khẩn cấp/check-in).</summary>
    [HttpGet("settings")]
    [ProducesResponseType(typeof(SafetySettingsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSettings()
        => Ok(await _safetyService.GetSettingsAsync(CurrentUserId));

    /// <summary>Cập nhật cài đặt an toàn.</summary>
    [HttpPut("settings")]
    [ProducesResponseType(typeof(SafetySettingsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateSafetySettingsDto dto)
        => Ok(await _safetyService.UpdateSettingsAsync(CurrentUserId, dto));

    /// <summary>Đặt mã PIN (4-6 số) để khóa app.</summary>
    [HttpPost("pin/setup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetupPin([FromBody] SetupPinDto dto)
    {
        var validation = await _setupPinValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        await _safetyService.SetupPinAsync(CurrentUserId, dto);
        return Ok(new { message = "Đặt mã PIN thành công." });
    }

    /// <summary>Quên PIN: gửi mã OTP qua email để đặt lại.</summary>
    [HttpPost("pin/forgot")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPin([FromBody] ForgotPinDto dto)
    {
        var message = await _safetyService.ForgotPinAsync(CurrentUserId, dto);
        return Ok(new { message });
    }

    /// <summary>Xác thực OTP quên PIN. Thành công thì PIN cũ bị gỡ, đặt lại PIN qua /pin/setup.</summary>
    [HttpPost("pin/verify-otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyPinOtp([FromBody] VerifyPinOtpDto dto)
    {
        var success = await _safetyService.VerifyPinOtpAsync(CurrentUserId, dto);
        return Ok(new { success, tempToken = (string?)null });
    }

    /// <summary>Check-in an toàn ("safe" | "help").</summary>
    [HttpPost("checkin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkin([FromBody] CheckinDto dto)
    {
        var validation = await _checkinValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        await _safetyService.CheckinAsync(CurrentUserId, dto);
        return Ok(new { confirmed = true });
    }

    /// <summary>Lấy liên hệ khẩn cấp + tin nhắn báo động.</summary>
    [HttpGet("emergency")]
    [ProducesResponseType(typeof(EmergencyDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmergency()
        => Ok(await _safetyService.GetEmergencyAsync(CurrentUserId));

    /// <summary>Cập nhật liên hệ khẩn cấp + tin nhắn báo động (thay toàn bộ danh sách).</summary>
    [HttpPut("emergency")]
    [ProducesResponseType(typeof(EmergencyDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpsertEmergency([FromBody] UpsertEmergencyDto dto)
        => Ok(await _safetyService.UpsertEmergencyAsync(CurrentUserId, dto));
}
