using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin/users")]
public class AdminUsersController : ApiControllerBase
{
    private readonly IAdminUserService _adminUserService;

    public AdminUsersController(IAdminUserService adminUserService)
        => _adminUserService = adminUserService;

    /// <summary>Danh sách user (phân trang, lọc theo search + status).</summary>
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? status = null)
        => Ok(await _adminUserService.ListAsync(search, status, page, pageSize));

    /// <summary>Thao tác hàng loạt: action = ban | unban | revoke-sessions.</summary>
    [HttpPost("bulk-action")]
    public async Task<IActionResult> BulkAction([FromBody] BulkActionDto dto)
        => Ok(await _adminUserService.BulkActionAsync(CurrentUserId, dto));

    /// <summary>Chi tiết user.</summary>
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> Detail(Guid userId)
        => Ok(await _adminUserService.GetDetailAsync(userId));

    /// <summary>Đổi trạng thái user (Active/Inactive/Banned/PendingVerification).</summary>
    [HttpPatch("{userId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid userId, [FromBody] UpdateUserStatusDto dto)
        => Ok(await _adminUserService.UpdateStatusAsync(CurrentUserId, userId, dto));

    /// <summary>Danh sách ghi chú nội bộ về user.</summary>
    [HttpGet("{userId:guid}/notes")]
    public async Task<IActionResult> GetNotes(Guid userId)
        => Ok(await _adminUserService.GetNotesAsync(userId));

    /// <summary>Thêm ghi chú nội bộ về user.</summary>
    [HttpPost("{userId:guid}/notes")]
    public async Task<IActionResult> AddNote(Guid userId, [FromBody] AddNoteDto dto)
        => Ok(await _adminUserService.AddNoteAsync(CurrentUserId, userId, dto));

    /// <summary>Đặt lại mật khẩu cho user (thu hồi mọi phiên).</summary>
    [HttpPost("{userId:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid userId, [FromBody] AdminResetPasswordDto dto)
    {
        await _adminUserService.ResetPasswordAsync(CurrentUserId, userId, dto);
        return Ok(new { message = "Đã đặt lại mật khẩu." });
    }

    /// <summary>Thu hồi mọi phiên đăng nhập của user.</summary>
    [HttpPost("{userId:guid}/revoke-sessions")]
    public async Task<IActionResult> RevokeSessions(Guid userId)
    {
        await _adminUserService.RevokeSessionsAsync(CurrentUserId, userId);
        return Ok(new { message = "Đã thu hồi các phiên đăng nhập." });
    }
}
