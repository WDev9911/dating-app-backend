using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.Application.DTOs.Discovery;
using SameMess.Application.DTOs.Search;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/search")]
public class SearchController : ApiControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService) => _searchService = searchService;

    /// <summary>Lấy các tùy chọn bộ lọc (sở thích, thành phố, giới tính, tính cách).</summary>
    [HttpGet("filters")]
    [ProducesResponseType(typeof(SearchFiltersDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilters()
        => Ok(await _searchService.GetFiltersAsync(CurrentUserId));

    /// <summary>
    /// Tìm hồ sơ theo bộ lọc tường minh.
    /// Query: gender, city, minAge, maxAge, interests (lặp nhiều lần), distanceKm, sort (distance|newest|age).
    /// </summary>
    [HttpGet("results")]
    [ProducesResponseType(typeof(List<DiscoveryProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResults([FromQuery] SearchQueryDto query)
        => Ok(await _searchService.SearchAsync(CurrentUserId, query));
}
