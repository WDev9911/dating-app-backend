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

    /// <summary>
    /// Xoá sạch mọi dữ liệu của user. Các bảng RESTRICT (Swipe/Match/Block/Report/Conversation/
    /// Message-by-sender) + bảng không có FK (MeetupProposal/NudgeDismissal/UserXp/DailyQuest) được
    /// xoá thủ công trước; phần còn lại xoá theo CASCADE khi xoá hàng auth."Users".
    /// </summary>
    public async Task PurgeAsync(Guid userId)
    {
        const string matchSub = "(SELECT \"Id\" FROM matching.\"Matches\" WHERE \"UserAId\" = {0} OR \"UserBId\" = {0})";
        var convSub = $"(SELECT \"Id\" FROM chat.\"Conversations\" WHERE \"MatchId\" IN {matchSub})";

        // Xoá lần lượt (mỗi câu 1 lệnh vì Npgsql không cho nhiều statement có tham số),
        // trong cùng 1 transaction để an toàn. Phần còn lại tự xoá theo ON DELETE CASCADE.
        var statements = new[]
        {
            $"DELETE FROM chat.\"MeetupProposals\" WHERE \"ProposerId\" = {{0}} OR \"ConversationId\" IN {convSub}",
            $"DELETE FROM chat.\"NudgeDismissals\" WHERE \"UserId\" = {{0}} OR \"ConversationId\" IN {convSub}",
            $"DELETE FROM chat.\"Messages\" WHERE \"SenderId\" = {{0}} OR \"ConversationId\" IN {convSub}",
            $"DELETE FROM chat.\"Conversations\" WHERE \"MatchId\" IN {matchSub}",
            $"DELETE FROM billing.\"DatePassOrders\" WHERE \"BuyerId\" = {{0}} OR \"MatchId\" IN {matchSub}",
            $"DELETE FROM gamification.\"MatchPlants\" WHERE \"MatchId\" IN {matchSub}",
            "DELETE FROM matching.\"Matches\" WHERE \"UserAId\" = {0} OR \"UserBId\" = {0}",
            "DELETE FROM matching.\"Swipes\" WHERE \"SwiperId\" = {0} OR \"TargetUserId\" = {0}",
            "DELETE FROM safety.\"Blocks\" WHERE \"BlockerId\" = {0} OR \"BlockedId\" = {0}",
            "DELETE FROM safety.\"Reports\" WHERE \"ReporterId\" = {0} OR \"ReportedId\" = {0}",
            "DELETE FROM gamification.\"UserXp\" WHERE \"UserId\" = {0}",
            "DELETE FROM gamification.\"DailyQuestCompletions\" WHERE \"UserId\" = {0}",
            "DELETE FROM auth.\"Users\" WHERE \"Id\" = {0}",
        };

        await using var tx = await _context.Database.BeginTransactionAsync();
        foreach (var s in statements)
            await _context.Database.ExecuteSqlRawAsync(s, userId);
        await tx.CommitAsync();
    }
}
