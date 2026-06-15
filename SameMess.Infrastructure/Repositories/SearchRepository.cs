using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly AppDbContext _context;

    public SearchRepository(AppDbContext context) => _context = context;

    public async Task<List<UserProfile>> SearchAsync(
        Guid excludeUserId,
        IReadOnlyCollection<Guid> excludeUserIds,
        string? gender,
        string? city,
        DateOnly? minBirthDate,
        DateOnly? maxBirthDate,
        IReadOnlyCollection<Guid> interestIds,
        int fetchLimit)
    {
        var query = _context.UserProfiles
            .Include(p => p.User).ThenInclude(u => u.Photos)
            .Include(p => p.User).ThenInclude(u => u.Preference)
            .Where(p => p.IsProfileCompleted
                        && p.UserId != excludeUserId
                        && !excludeUserIds.Contains(p.UserId));

        if (!string.IsNullOrWhiteSpace(gender))
            query = query.Where(p => p.Gender == gender);

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(p => p.Location != null && EF.Functions.ILike(p.Location, $"%{city}%"));

        if (minBirthDate.HasValue)
            query = query.Where(p => p.DateOfBirth != null && p.DateOfBirth >= minBirthDate);

        if (maxBirthDate.HasValue)
            query = query.Where(p => p.DateOfBirth != null && p.DateOfBirth <= maxBirthDate);

        if (interestIds.Count > 0)
        {
            var matchedUserIds = _context.UserInterests
                .Where(ui => interestIds.Contains(ui.InterestId))
                .Select(ui => ui.UserId);
            query = query.Where(p => matchedUserIds.Contains(p.UserId));
        }

        return await query.Take(fetchLimit).ToListAsync();
    }

    public async Task<List<string>> GetDistinctCitiesAsync(int limit) =>
        await _context.UserProfiles
            .Where(p => p.IsProfileCompleted && p.Location != null && p.Location != "")
            .Select(p => p.Location!)
            .Distinct()
            .OrderBy(c => c)
            .Take(limit)
            .ToListAsync();
}
