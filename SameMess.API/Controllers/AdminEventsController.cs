using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin/events")]
public class AdminEventsController : ApiControllerBase
{
    private readonly IAdminEventService _service;

    public AdminEventsController(IAdminEventService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _service.ListAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id) => Ok(await _service.GetAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EventPayloadDto dto)
        => Ok(await _service.CreateAsync(CurrentUserId, dto));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EventPayloadDto dto)
        => Ok(await _service.UpdateAsync(CurrentUserId, id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
        => Ok(await _service.SetPublishedAsync(CurrentUserId, id, true));

    [HttpPost("{id:guid}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id)
        => Ok(await _service.SetPublishedAsync(CurrentUserId, id, false));

    [HttpGet("{id:guid}/registrations")]
    public async Task<IActionResult> Registrations(Guid id)
        => Ok(await _service.GetRegistrationsAsync(id));
}
