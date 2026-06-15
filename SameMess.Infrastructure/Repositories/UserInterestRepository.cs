using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class UserInterestRepository : BaseRepository<UserInterest>, IUserInterestRepository
{
    public UserInterestRepository(AppDbContext context) : base(context) { }

    public async Task<List<UserInterest>> GetByUserIdAsync(Guid userId) =>
        await _dbSet.Include(ui => ui.Interest)
            .Where(ui => ui.UserId == userId)
            .ToListAsync();

    public async Task ReplaceForUserAsync(Guid userId, IReadOnlyCollection<Guid> interestIds)
    {
        var existing = await _dbSet.Where(ui => ui.UserId == userId).ToListAsync();
        _dbSet.RemoveRange(existing);

        foreach (var interestId in interestIds.Distinct())
        {
            await _dbSet.AddAsync(new UserInterest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                InterestId = interestId,
            });
        }
    }
}
