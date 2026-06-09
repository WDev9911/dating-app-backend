using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface ISwipeRepository : IBaseRepository<Swipe>
{
    Task<Swipe?> GetAsync(Guid swiperId, Guid targetUserId);
    Task<List<Guid>> GetSwipedTargetIdsAsync(Guid swiperId);

    /// <summary>Những swipe Like (thường) hướng tới userId mà userId CHƯA swipe lại.</summary>
    Task<List<Swipe>> GetLikersAsync(Guid userId);

    /// <summary>Những swipe SuperLike hướng tới userId mà userId CHƯA swipe lại (mục riêng).</summary>
    Task<List<Swipe>> GetSuperLikersAsync(Guid userId);

    /// <summary>Lần swipe gần nhất của userId (để hoàn tác).</summary>
    Task<Swipe?> GetLastSwipeAsync(Guid userId);

    /// <summary>Số lượt Like/SuperLike user đã thực hiện kể từ mốc UTC (đếm giới hạn/ngày cho gói Free).</summary>
    Task<int> CountLikesSinceAsync(Guid userId, DateTime sinceUtc);
}
