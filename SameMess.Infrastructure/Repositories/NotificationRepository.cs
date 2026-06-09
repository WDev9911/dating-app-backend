using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task<List<Notification>> GetForUserAsync(Guid userId, int limit) =>
        await _dbSet
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync();

    public async Task<int> CountUnreadAsync(Guid userId) =>
        await _dbSet.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task MarkAllReadAsync(Guid userId) =>
        await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
}
