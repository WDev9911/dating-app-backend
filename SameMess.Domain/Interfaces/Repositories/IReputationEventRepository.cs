using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IReputationEventRepository : IBaseRepository<ReputationEvent>
{
    Task<List<ReputationEvent>> GetByUserAsync(Guid userId);
    Task<bool> ExistsAsync(Guid userId, string type);
}
