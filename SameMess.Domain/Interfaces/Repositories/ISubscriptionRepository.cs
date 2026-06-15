using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    Task<Subscription?> GetByUserAsync(Guid userId);

    /// <summary>Danh sách thuê bao, lọc theo planCode (null = tất cả) và còn hiệu lực (activeOnly).</summary>
    Task<List<Subscription>> GetSubscribersAsync(string? planCode, bool activeOnly);
}
