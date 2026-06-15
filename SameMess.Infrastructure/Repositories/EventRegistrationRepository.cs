using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class EventRegistrationRepository : BaseRepository<EventRegistration>, IEventRegistrationRepository
{
    public EventRegistrationRepository(AppDbContext context) : base(context) { }

    public async Task<EventRegistration?> GetAsync(Guid eventId, Guid userId) =>
        await _dbSet.FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

    public async Task<int> CountByEventAsync(Guid eventId) =>
        await _dbSet.CountAsync(r => r.EventId == eventId && r.Status != EventRegistrationStatus.Cancelled);

    public async Task<List<EventRegistration>> GetUserRegistrationsAsync(Guid userId) =>
        await _dbSet.Include(r => r.Event)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RegisteredAt)
            .ToListAsync();

    public async Task<List<Guid>> GetRegisteredEventIdsAsync(Guid userId) =>
        await _dbSet.Where(r => r.UserId == userId && r.Status != EventRegistrationStatus.Cancelled)
            .Select(r => r.EventId)
            .ToListAsync();

    public async Task<List<EventRegistration>> GetByEventAsync(Guid eventId) =>
        await _dbSet.Where(r => r.EventId == eventId)
            .OrderByDescending(r => r.RegisteredAt)
            .ToListAsync();

    public async Task<Dictionary<Guid, int>> GetCountsByEventsAsync(IReadOnlyCollection<Guid> eventIds) =>
        await _dbSet
            .Where(r => eventIds.Contains(r.EventId) && r.Status != EventRegistrationStatus.Cancelled)
            .GroupBy(r => r.EventId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);
}
