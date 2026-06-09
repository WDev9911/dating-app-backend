using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber) =>
        await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

    public async Task<bool> EmailExistsAsync(string email) =>
        await _dbSet.AnyAsync(u => u.Email == email.ToLower().Trim());

    public async Task<bool> PhoneExistsAsync(string phoneNumber) =>
        await _dbSet.AnyAsync(u => u.PhoneNumber == phoneNumber);

    public async Task<User?> GetWithProfileAsync(Guid userId) =>
        await _dbSet.Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId);

    public async Task<User?> GetFullProfileAsync(Guid userId) =>
        await _dbSet
            .Include(u => u.Profile)
            .Include(u => u.Preference)
            .Include(u => u.Photos.OrderBy(p => p.OrderIndex))
            .FirstOrDefaultAsync(u => u.Id == userId);

    public async Task<List<User>> GetWithProfileByIdsAsync(IEnumerable<Guid> ids) =>
        await _dbSet
            .Include(u => u.Profile)
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();

    public async Task<List<User>> GetWithProfileAndPhotosByIdsAsync(IEnumerable<Guid> ids) =>
        await _dbSet
            .Include(u => u.Profile)
            .Include(u => u.Photos.OrderBy(p => p.OrderIndex))
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();

    public async Task<List<User>> GetPendingFaceVerificationsAsync() =>
        await _dbSet
            .Include(u => u.Profile)
            .Include(u => u.Photos.OrderBy(p => p.OrderIndex))
            .Where(u => u.Profile != null
                        && u.Profile.VerificationStatus == Domain.Enums.VerificationStatus.Pending)
            .ToListAsync();
}
