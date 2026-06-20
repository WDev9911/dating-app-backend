using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.DatePass;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;

namespace SameMess.API.Controllers;

[Authorize(Roles = UserRole.Admin)]
[Route("api/admin/combos")]
public class AdminCombosController : ApiControllerBase
{
    private readonly IDatePassService _service;

    public AdminCombosController(IDatePassService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _service.AdminListCombosAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ComboPayloadDto dto)
        => Ok(await _service.AdminCreateComboAsync(dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.AdminDeleteComboAsync(id);
        return NoContent();
    }
}
