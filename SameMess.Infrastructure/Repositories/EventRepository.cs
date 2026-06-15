using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class EventRepository : BaseRepository<Event>, IEventRepository
{
    public EventRepository(AppDbContext context) : base(context) { }

    public async Task<List<Event>> GetPublishedAsync() =>
        await _dbSet.Where(e => e.IsPublished)
            .OrderBy(e => e.StartAt)
            .ToListAsync();
}
