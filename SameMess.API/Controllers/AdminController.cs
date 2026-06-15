using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
using SameMess.Application.DTOs.Admin;
using SameMess.Application.DTOs.Safety;
using SameMess.Application.DTOs.Verification;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin")]
public class AdminController : ApiControllerBase
{
    private readonly IAdminReportService _adminReportService;
    private readonly IProfileVerificationService _verificationService;
    private readonly IAuditService _auditService;
    private readonly IValidator<UpdateReportStatusDto> _updateValidator;

    public AdminController(
        IAdminReportService adminReportService,
        IProfileVerificationService verificationService,
        IAuditService auditService,
        IValidator<UpdateReportStatusDto> updateValidator)
    {
        _adminReportService = adminReportService;
        _verificationService = verificationService;
        _auditService = auditService;
        _updateValidator = updateValidator;
    }

    // ===================== Reports =====================

    /// <summary>Danh sách report. Lọc: ?status=Pending (bỏ trống = tất cả).</summary>
    [HttpGet("reports")]
    public async Task<IActionResult> GetReports([FromQuery] string? status)
        => Ok(await _adminReportService.GetReportsAsync(status));

    /// <summary>Chi tiết một report.</summary>
    [HttpGet("reports/{id:guid}")]
    public async Task<IActionResult> GetReport(Guid id)
        => Ok(await _adminReportService.GetReportDetailAsync(id));

    /// <summary>(Cũ) Cập nhật trạng thái report + tùy chọn ban.</summary>
    [HttpPut("reports/{id:guid}")]
    public async Task<IActionResult> ResolveReportLegacy(Guid id, [FromBody] UpdateReportStatusDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        await _adminReportService.ResolveAsync(id, dto);
        return NoContent();
    }

    /// <summary>Gán report cho một admin xử lý.</summary>
    [HttpPost("reports/{id:guid}/assign")]
    public async Task<IActionResult> AssignReport(Guid id, [FromBody] AssignReportDto dto)
    {
        await _adminReportService.AssignAsync(id, dto.AdminId);
        await _auditService.LogAsync(CurrentUserId, "report.assign", "Report", id, dto.AdminId.ToString());
        return Ok(new { message = "Đã gán report." });
    }

    /// <summary>Xử lý report (Resolved). action="ban" để ban người bị báo cáo.</summary>
    [HttpPost("reports/{id:guid}/resolve")]
    public async Task<IActionResult> ResolveReport(Guid id, [FromBody] ResolveReportDto dto)
    {
        await _adminReportService.ResolveReportAsync(id, dto.Action, dto.Note);
        await _auditService.LogAsync(CurrentUserId, "report.resolve", "Report", id, dto.Action);
        return Ok(new { message = "Đã xử lý report." });
    }

    /// <summary>Bỏ qua report (Dismissed).</summary>
    [HttpPost("reports/{id:guid}/dismiss")]
    public async Task<IActionResult> DismissReport(Guid id, [FromBody] DismissReportDto dto)
    {
        await _adminReportService.DismissReportAsync(id, dto.Note);
        await _auditService.LogAsync(CurrentUserId, "report.dismiss", "Report", id);
        return Ok(new { message = "Đã bỏ qua report." });
    }

    // ===================== Verifications =====================

    /// <summary>Danh sách hồ sơ chờ duyệt xác minh khuôn mặt.</summary>
    [HttpGet("verifications")]
    public async Task<IActionResult> GetPendingVerifications()
        => Ok(await _verificationService.GetPendingAsync());

    /// <summary>Chi tiết một hồ sơ xác minh.</summary>
    [HttpGet("verifications/{userId:guid}")]
    public async Task<IActionResult> GetVerification(Guid userId)
        => Ok(await _verificationService.GetDetailAsync(userId));

    /// <summary>(Cũ) Duyệt/từ chối qua body { approve }.</summary>
    [HttpPut("verifications/{userId:guid}")]
    public async Task<IActionResult> ReviewVerification(Guid userId, [FromBody] ReviewVerificationDto dto)
        => Ok(await _verificationService.ReviewAsync(userId, dto.Approve));

    /// <summary>Duyệt xác minh khuôn mặt.</summary>
    [HttpPost("verifications/{userId:guid}/approve")]
    public async Task<IActionResult> ApproveVerification(Guid userId, [FromBody] ApproveVerificationDto? dto)
    {
        var result = await _verificationService.ReviewAsync(userId, true);
        await _auditService.LogAsync(CurrentUserId, "verification.approve", "User", userId, dto?.Note);
        return Ok(result);
    }

    /// <summary>Từ chối xác minh khuôn mặt.</summary>
    [HttpPost("verifications/{userId:guid}/reject")]
    public async Task<IActionResult> RejectVerification(Guid userId, [FromBody] RejectVerificationDto? dto)
    {
        var result = await _verificationService.ReviewAsync(userId, false);
        await _auditService.LogAsync(CurrentUserId, "verification.reject", "User", userId, dto?.Reason);
        return Ok(result);
    }
}
