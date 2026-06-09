using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class ReportRepository : BaseRepository<Report>, IReportRepository
{
    public ReportRepository(AppDbContext context) : base(context) { }

    public async Task<List<Report>> GetByStatusAsync(string? status) =>
        await _dbSet
            .Where(r => status == null || r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
}
