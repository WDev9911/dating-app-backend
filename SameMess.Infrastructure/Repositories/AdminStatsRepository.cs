using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Domain.Models;
using SameMess.Domain.Enums;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class AdminStatsRepository : IAdminStatsRepository
{
    private readonly AppDbContext _context;

    public AdminStatsRepository(AppDbContext context) => _context = context;

    public async Task<DashboardSnapshot> GetDashboardAsync()
    {
        var now = DateTime.UtcNow;
        var todayStart = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        var weekAgo = now.AddDays(-7);

        return new DashboardSnapshot
        {
            TotalUsers = await _context.Users.CountAsync(),
            ActiveUsers = await _context.Users.CountAsync(u => u.Status == UserStatus.Active),
            BannedUsers = await _context.Users.CountAsync(u => u.Status == UserStatus.Banned),
            PendingVerifications = await _context.UserProfiles.CountAsync(p => p.VerificationStatus == VerificationStatus.Pending),
            NewUsersToday = await _context.Users.CountAsync(u => u.CreatedAt >= todayStart),
            NewUsers7Days = await _context.Users.CountAsync(u => u.CreatedAt >= weekAgo),
            TotalMatches = await _context.Matches.CountAsync(),
            PendingReports = await _context.Reports.CountAsync(r => r.Status == ReportStatus.Pending),
            ActiveSubscriptions = await _context.Subscriptions.CountAsync(s => s.ExpiresAt > now),
            RevenueVnd = await _context.PaymentOrders
                .Where(o => o.Status == PaymentStatus.Paid)
                .SumAsync(o => (long)o.AmountVnd),
        };
    }

    public async Task<List<DailyCount>> GetDailySeriesAsync(string type, DateTime fromUtc, DateTime toUtc)
    {
        List<(DateTime Day, long Value)> raw;

        switch (type?.ToLowerInvariant())
        {
            case "matches":
                raw = (await _context.Matches
                    .Where(m => m.CreatedAt >= fromUtc && m.CreatedAt < toUtc)
                    .GroupBy(m => m.CreatedAt.Date)
                    .Select(g => new { g.Key, Value = (long)g.Count() })
                    .ToListAsync())
                    .Select(x => (x.Key, x.Value)).ToList();
                break;

            case "revenue":
                raw = (await _context.PaymentOrders
                    .Where(o => o.Status == PaymentStatus.Paid && o.PaidAt != null
                                && o.PaidAt >= fromUtc && o.PaidAt < toUtc)
                    .GroupBy(o => o.PaidAt!.Value.Date)
                    .Select(g => new { g.Key, Value = g.Sum(o => (long)o.AmountVnd) })
                    .ToListAsync())
                    .Select(x => (x.Key, x.Value)).ToList();
                break;

            default: // signups
                raw = (await _context.Users
                    .Where(u => u.CreatedAt >= fromUtc && u.CreatedAt < toUtc)
                    .GroupBy(u => u.CreatedAt.Date)
                    .Select(g => new { g.Key, Value = (long)g.Count() })
                    .ToListAsync())
                    .Select(x => (x.Key, x.Value)).ToList();
                break;
        }

        return raw
            .Select(x => new DailyCount { Date = DateOnly.FromDateTime(x.Day), Value = x.Value })
            .OrderBy(d => d.Date)
            .ToList();
    }
}
