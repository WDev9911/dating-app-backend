using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin/settings")]
public class AdminSettingsController : ApiControllerBase
{
    private readonly IAdminSettingsService _service;

    public AdminSettingsController(IAdminSettingsService service) => _service = service;

    /// <summary>Lấy toàn bộ cấu hình hệ thống + feature flags.</summary>
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.GetAsync());

    /// <summary>Cập nhật cấu hình (upsert từng key trong settings).</summary>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSettingsDto dto)
        => Ok(await _service.UpdateSettingsAsync(CurrentUserId, dto));

    /// <summary>Bật/tắt một feature flag theo key.</summary>
    [HttpPut("~/api/admin/feature-flags/{key}")]
    public async Task<IActionResult> SetFeatureFlag(string key, [FromBody] UpdateFeatureFlagDto dto)
    {
        await _service.SetFeatureFlagAsync(CurrentUserId, key, dto.Value);
        return Ok(new { key, value = dto.Value });
    }
}
