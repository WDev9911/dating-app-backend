using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class FeatureFlagRepository : IFeatureFlagRepository
{
    private readonly AppDbContext _context;

    public FeatureFlagRepository(AppDbContext context) => _context = context;

    public async Task<List<FeatureFlag>> GetAllAsync() =>
        await _context.FeatureFlags.OrderBy(f => f.Key).ToListAsync();

    public async Task UpsertAsync(string key, bool isEnabled)
    {
        var existing = await _context.FeatureFlags.FirstOrDefaultAsync(f => f.Key == key);
        if (existing is null)
        {
            await _context.FeatureFlags.AddAsync(new FeatureFlag { Key = key, IsEnabled = isEnabled, UpdatedAt = DateTime.UtcNow });
        }
        else
        {
            existing.IsEnabled = isEnabled;
            existing.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
