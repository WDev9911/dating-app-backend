using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Connection;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/venues")]
public class VenuesController : ApiControllerBase
{
    private readonly IVenueService _venueService;

    public VenuesController(IVenueService venueService) => _venueService = venueService;

    /// <summary>Chi tiết một địa điểm (bấm vào thẻ quán đã chia sẻ trong chat).</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VenueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
        => Ok(await _venueService.GetByIdAsync(id));
}
