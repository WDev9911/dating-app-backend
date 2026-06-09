using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
using SameMess.Application.DTOs.Preference;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/preferences")]
public class PreferenceController : ApiControllerBase
{
    private readonly IPreferenceService _preferenceService;
    private readonly IValidator<UpdatePreferenceDto> _updatePreferenceValidator;

    public PreferenceController(
        IPreferenceService preferenceService,
        IValidator<UpdatePreferenceDto> updatePreferenceValidator)
    {
        _preferenceService = preferenceService;
        _updatePreferenceValidator = updatePreferenceValidator;
    }

    /// <summary>Lấy tiêu chí lọc của tôi (tạo mặc định nếu chưa có).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PreferenceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPreference()
        => Ok(await _preferenceService.GetMyPreferenceAsync(CurrentUserId));

    /// <summary>Cập nhật tiêu chí lọc (giới tính quan tâm, khoảng tuổi, khoảng cách).</summary>
    [HttpPut]
    [ProducesResponseType(typeof(PreferenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePreference([FromBody] UpdatePreferenceDto dto)
    {
        var validation = await _updatePreferenceValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        return Ok(await _preferenceService.UpdatePreferenceAsync(CurrentUserId, dto));
    }
}
