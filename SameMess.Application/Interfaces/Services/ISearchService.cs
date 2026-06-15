using SameMess.Application.DTOs.Discovery;
using SameMess.Application.DTOs.Search;

namespace SameMess.Application.Interfaces.Services;

public interface ISearchService
{
    Task<SearchFiltersDto> GetFiltersAsync(Guid userId);
    Task<List<DiscoveryProfileDto>> SearchAsync(Guid userId, SearchQueryDto query);
}
