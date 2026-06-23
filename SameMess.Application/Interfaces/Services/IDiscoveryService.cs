using SameMess.Application.DTOs.Discovery;

namespace SameMess.Application.Interfaces.Services;

public interface IDiscoveryService
{
    Task<List<DiscoveryProfileDto>> GetFeedAsync(Guid userId, int limit, bool includeSwiped = false);
}
