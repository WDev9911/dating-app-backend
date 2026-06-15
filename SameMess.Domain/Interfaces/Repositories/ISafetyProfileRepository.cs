using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface ISafetyProfileRepository : IBaseRepository<SafetyProfile>
{
    Task<SafetyProfile?> GetByUserIdAsync(Guid userId);
}
