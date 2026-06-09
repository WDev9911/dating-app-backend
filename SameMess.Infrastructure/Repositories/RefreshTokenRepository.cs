using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash) =>
        await _dbSet.Include(r => r.User)
            .ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        var tokens = await _dbSet
            .Where(r => r.UserId == userId && r.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
            token.RevokedAt = DateTime.UtcNow;
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId) =>
        await _dbSet
            .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
}
