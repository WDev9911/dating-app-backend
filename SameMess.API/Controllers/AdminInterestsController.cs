using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin/interests")]
public class AdminInterestsController : ApiControllerBase
{
    private readonly IAdminInterestService _service;

    public AdminInterestsController(IAdminInterestService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _service.ListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InterestPayloadDto dto)
        => Ok(await _service.CreateAsync(CurrentUserId, dto));

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] InterestPayloadDto dto)
        => Ok(await _service.UpdateAsync(CurrentUserId, id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }

    /// <summary>Tạo nhãn nhóm sở thích (groupName dùng khi tạo interest).</summary>
    [HttpPost("~/api/admin/interest-groups")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateInterestGroupDto dto)
        => Ok(new { name = await _service.CreateGroupAsync(CurrentUserId, dto.Name) });
}
