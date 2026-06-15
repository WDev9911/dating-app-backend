using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IInterestRepository : IBaseRepository<Interest>
{
    Task<List<Interest>> GetActiveAsync();
    Task<List<Interest>> GetByIdsAsync(IReadOnlyCollection<Guid> ids);
}
