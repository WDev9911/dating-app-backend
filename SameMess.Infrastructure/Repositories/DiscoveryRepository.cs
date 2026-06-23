using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class DiscoveryRepository : IDiscoveryRepository
{
    private readonly AppDbContext _context;

    public DiscoveryRepository(AppDbContext context) => _context = context;

    public async Task<List<UserProfile>> GetCandidatesAsync(
        Guid excludeUserId,
        IReadOnlyCollection<Guid> excludeUserIds,
        string? requiredGender,
        string? viewerGender,
        int viewerAge,
        DateOnly minBirthDate,
        DateOnly maxBirthDate,
        double minLat,
        double maxLat,
        double minLon,
        double maxLon,
        int fetchLimit)
    {
        var query = _context.UserProfiles
            .AsNoTracking()
            .Include(p => p.User)
                .ThenInclude(u => u.Photos)
            .Include(p => p.User)
                .ThenInclude(u => u.Preference)
            .Where(p => p.UserId != excludeUserId)
            .Where(p => !excludeUserIds.Contains(p.UserId))
            .Where(p => p.User.Status == UserStatus.Active)   // bỏ user bị ban/khoá
            .Where(p => p.IsProfileCompleted)
            .Where(p => p.Latitude != null && p.Longitude != null)
            .Where(p => p.Latitude >= minLat && p.Latitude <= maxLat)
            .Where(p => p.Longitude >= minLon && p.Longitude <= maxLon)
            .Where(p => p.DateOfBirth != null
                        && p.DateOfBirth >= minBirthDate
                        && p.DateOfBirth <= maxBirthDate);

        // Chiều 1: họ đúng giới tính tôi muốn xem
        if (requiredGender != null)
            query = query.Where(p => p.Gender == requiredGender);

        // Chiều 2: tôi cũng nằm trong tiêu chí của họ (giới tính + khoảng tuổi).
        // Nếu họ chưa có preference → coi như chấp nhận (Everyone / 18–99).
        query = query.Where(p =>
            p.User.Preference == null
            || (
                (p.User.Preference.InterestedInGender == GenderPreference.Everyone
                 || viewerGender == null
                 || p.User.Preference.InterestedInGender == viewerGender)
                && viewerAge >= p.User.Preference.MinAge
                && viewerAge <= p.User.Preference.MaxAge
            ));

        return await query
            .Take(fetchLimit)
            .ToListAsync();
    }
}
