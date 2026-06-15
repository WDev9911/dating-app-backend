using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class AppSettingRepository : IAppSettingRepository
{
    private readonly AppDbContext _context;

    public AppSettingRepository(AppDbContext context) => _context = context;

    public async Task<List<AppSetting>> GetAllAsync() =>
        await _context.AppSettings.OrderBy(s => s.Key).ToListAsync();

    public async Task UpsertAsync(string key, string value)
    {
        var existing = await _context.AppSettings.FirstOrDefaultAsync(s => s.Key == key);
        if (existing is null)
        {
            await _context.AppSettings.AddAsync(new AppSetting { Key = key, Value = value, UpdatedAt = DateTime.UtcNow });
        }
        else
        {
            existing.Value = value;
            existing.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
