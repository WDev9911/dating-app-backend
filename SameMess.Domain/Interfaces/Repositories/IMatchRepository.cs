using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IMatchRepository : IBaseRepository<Match>
{
    /// <summary>Lấy match theo cặp đã chuẩn hóa (userAId &lt; userBId), bất kể IsActive.</summary>
    Task<Match?> GetByPairAsync(Guid userAId, Guid userBId);

    Task<List<Match>> GetActiveForUserAsync(Guid userId);

    Task<Match?> GetByIdForUserAsync(Guid matchId, Guid userId);
}
