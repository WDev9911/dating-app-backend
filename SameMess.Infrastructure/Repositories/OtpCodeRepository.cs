using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class OtpCodeRepository : BaseRepository<OtpCode>, IOtpCodeRepository
{
    public OtpCodeRepository(AppDbContext context) : base(context) { }

    public async Task<OtpCode?> GetValidOtpAsync(string email, string code, string purpose) =>
        await _dbSet.FirstOrDefaultAsync(o =>
            o.Email == email.ToLower().Trim() &&
            o.Code == code &&
            o.Purpose == purpose &&
            !o.IsUsed &&
            o.ExpiresAt > DateTime.UtcNow);

    public async Task InvalidateAllOtpsForEmailAsync(string email, string purpose)
    {
        var otps = await _dbSet
            .Where(o => o.Email == email.ToLower().Trim() && o.Purpose == purpose && !o.IsUsed)
            .ToListAsync();

        foreach (var otp in otps)
            otp.IsUsed = true;
    }
}
