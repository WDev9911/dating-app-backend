using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IReputationScoreRepository : IBaseRepository<ReputationScore>
{
    Task<ReputationScore?> GetByUserAsync(Guid userId);
    Task<List<ReputationScore>> GetByUserIdsAsync(IEnumerable<Guid> userIds);
}
