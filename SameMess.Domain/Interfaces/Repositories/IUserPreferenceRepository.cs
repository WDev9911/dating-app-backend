using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IUserPreferenceRepository : IBaseRepository<UserPreference>
{
    Task<UserPreference?> GetByUserIdAsync(Guid userId);
}
