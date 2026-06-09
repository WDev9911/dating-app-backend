using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IMatchPlantRepository : IBaseRepository<MatchPlant>
{
    Task<MatchPlant?> GetByMatchIdAsync(Guid matchId);
}
