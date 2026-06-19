using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin/venues")]
public class AdminVenuesController : ApiControllerBase
{
    private readonly IVenueService _venueService;

    public AdminVenuesController(IVenueService venueService) => _venueService = venueService;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool includeInactive = true)
        => Ok(await _venueService.ListAsync(includeInactive));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VenuePayloadDto dto)
        => Ok(await _venueService.CreateAsync(CurrentUserId, dto));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] VenuePayloadDto dto)
        => Ok(await _venueService.UpdateAsync(CurrentUserId, id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _venueService.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }
}
