using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IUserTaskProgressRepository : IBaseRepository<UserTaskProgress>
{
    Task<UserTaskProgress?> GetAsync(Guid userId, string taskCode, string periodKey);

    /// <summary>Tiến độ của user thuộc các kỳ đang xét (today / tuần này / "ALL").</summary>
    Task<List<UserTaskProgress>> GetByUserAndKeysAsync(Guid userId, IEnumerable<string> periodKeys);
}
