using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<List<Notification>> GetForUserAsync(Guid userId, int limit);
    Task<int> CountUnreadAsync(Guid userId);
    Task MarkAllReadAsync(Guid userId);
}
