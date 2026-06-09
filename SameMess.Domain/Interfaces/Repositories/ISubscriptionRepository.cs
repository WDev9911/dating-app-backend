using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    Task<Subscription?> GetByUserAsync(Guid userId);
}
