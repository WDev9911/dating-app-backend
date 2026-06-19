using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Connection;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/meetup")]
public class MeetupController : ApiControllerBase
{
    private readonly IVenueService _venueService;

    public MeetupController(IVenueService venueService) => _venueService = venueService;

    /// <summary>
    /// Gợi ý địa điểm hẹn hò gần điểm giữa của cặp match.
    /// Chỉ mở khi "Cây tình yêu" của cặp đạt Level ≥ 4 (nếu chưa → 403).
    /// </summary>
    [HttpGet("nearby/{matchId:guid}")]
    [ProducesResponseType(typeof(List<VenueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Nearby(Guid matchId, [FromQuery] string? category, [FromQuery] int? radiusKm)
        => Ok(await _venueService.GetNearbyForMatchAsync(CurrentUserId, matchId, category, radiusKm));
}
