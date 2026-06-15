using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin/photos")]
public class AdminPhotosController : ApiControllerBase
{
    private readonly IAdminPhotoService _photoService;

    public AdminPhotosController(IAdminPhotoService photoService) => _photoService = photoService;

    /// <summary>Danh sách ảnh theo trạng thái kiểm duyệt (mặc định Pending).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AdminPhotoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] string? status)
        => Ok(await _photoService.ListAsync(status));

    /// <summary>Duyệt ảnh.</summary>
    [HttpPost("{photoId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid photoId)
    {
        await _photoService.ApproveAsync(CurrentUserId, photoId);
        return Ok(new { message = "Đã duyệt ảnh." });
    }

    /// <summary>Từ chối ảnh (kèm lý do).</summary>
    [HttpPost("{photoId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid photoId, [FromBody] RejectPhotoDto? dto)
    {
        await _photoService.RejectAsync(CurrentUserId, photoId, dto?.Reason);
        return Ok(new { message = "Đã từ chối ảnh." });
    }
}
