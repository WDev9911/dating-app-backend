using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IUserXpRepository : IBaseRepository<UserXp>
{
    Task<UserXp?> GetByUserAsync(Guid userId);
}
