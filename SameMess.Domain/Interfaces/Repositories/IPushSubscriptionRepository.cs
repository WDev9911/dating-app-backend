using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IPushSubscriptionRepository : IBaseRepository<PushSubscription>
{
    Task<List<PushSubscription>> GetByUserAsync(Guid userId);
    Task<PushSubscription?> GetByEndpointAsync(Guid userId, string endpoint);
}
