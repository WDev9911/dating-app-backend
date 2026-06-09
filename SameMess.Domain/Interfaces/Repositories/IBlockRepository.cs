using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IBlockRepository : IBaseRepository<Block>
{
    Task<Block?> GetAsync(Guid blockerId, Guid blockedId);

    /// <summary>Có tồn tại block giữa 2 người theo bất kỳ chiều nào không.</summary>
    Task<bool> ExistsBetweenAsync(Guid user1, Guid user2);

    /// <summary>Tất cả userId liên quan block với tôi (tôi block họ HOẶC họ block tôi) — để ẩn 2 chiều.</summary>
    Task<List<Guid>> GetRelatedUserIdsAsync(Guid userId);

    Task<List<Block>> GetByBlockerAsync(Guid blockerId);
}
