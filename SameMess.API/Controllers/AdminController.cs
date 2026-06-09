using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
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
    private readonly IValidator<UpdateReportStatusDto> _updateValidator;

    public AdminController(
        IAdminReportService adminReportService,
        IProfileVerificationService verificationService,
        IValidator<UpdateReportStatusDto> updateValidator)
    {
        _adminReportService = adminReportService;
        _verificationService = verificationService;
        _updateValidator = updateValidator;
    }

    /// <summary>Danh sách report. Lọc theo trạng thái: ?status=Pending (bỏ trống = tất cả).</summary>
    [HttpGet("reports")]
    [ProducesResponseType(typeof(List<ReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetReports([FromQuery] string? status)
        => Ok(await _adminReportService.GetReportsAsync(status));

    /// <summary>Xử lý report: đổi trạng thái (Reviewed/Resolved/Dismissed) + tùy chọn ban người bị báo cáo.</summary>
    [HttpPut("reports/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveReport(Guid id, [FromBody] UpdateReportStatusDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        await _adminReportService.ResolveAsync(id, dto);
        return NoContent();
    }

    /// <summary>Danh sách hồ sơ chờ duyệt xác minh khuôn mặt (so khớp không chắc).</summary>
    [HttpGet("verifications")]
    [ProducesResponseType(typeof(List<PendingVerificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPendingVerifications()
        => Ok(await _verificationService.GetPendingAsync());

    /// <summary>Duyệt (Approve) / từ chối (Reject) xác minh khuôn mặt của một user.</summary>
    [HttpPut("verifications/{userId:guid}")]
    [ProducesResponseType(typeof(VerificationStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReviewVerification(Guid userId, [FromBody] ReviewVerificationDto dto)
        => Ok(await _verificationService.ReviewAsync(userId, dto.Approve));
}
