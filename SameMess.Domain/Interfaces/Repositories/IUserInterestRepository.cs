using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IUserInterestRepository : IBaseRepository<UserInterest>
{
    /// <summary>Sở thích của user (kèm thông tin Interest).</summary>
    Task<List<UserInterest>> GetByUserIdAsync(Guid userId);

    /// <summary>Thay toàn bộ sở thích của user bằng danh sách mới (xóa cũ, thêm mới).</summary>
    Task ReplaceForUserAsync(Guid userId, IReadOnlyCollection<Guid> interestIds);
}
