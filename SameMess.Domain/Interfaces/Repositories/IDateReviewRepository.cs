using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IDateReviewRepository : IBaseRepository<DateReview>
{
    /// <summary>Người này đã đánh giá buổi hẹn (đơn) đó chưa?</summary>
    Task<bool> ExistsAsync(Guid datePassOrderId, Guid reviewerId);

    /// <summary>Danh sách review VỀ một người (mới nhất trước).</summary>
    Task<List<DateReview>> GetForRevieweeAsync(Guid revieweeId);

    /// <summary>Số lượng + điểm trung bình review của một người.</summary>
    Task<(int Count, double Average)> GetSummaryAsync(Guid revieweeId);

    /// <summary>Trong tập đơn cho trước, đơn nào người này đã đánh giá rồi (để lọc phần "chờ đánh giá").</summary>
    Task<HashSet<Guid>> GetReviewedOrderIdsAsync(Guid reviewerId, IEnumerable<Guid> orderIds);
}
