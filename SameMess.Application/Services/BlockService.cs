using SameMess.Application.DTOs.Safety;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class BlockService : IBlockService
{
    private readonly IBlockRepository _blockRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly IReputationService _reputationService;

    public BlockService(
        IBlockRepository blockRepository,
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IReputationService reputationService)
    {
        _blockRepository = blockRepository;
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _reputationService = reputationService;
    }

    public async Task BlockAsync(Guid userId, Guid targetUserId)
    {
        if (targetUserId == userId)
            throw new BadRequestException("You cannot block yourself.");

        _ = await _userRepository.GetByIdAsync(targetUserId)
            ?? throw new NotFoundException("User", targetUserId);

        // Idempotent: nếu đã block thì không tạo trùng
        var isNewBlock = await _blockRepository.GetAsync(userId, targetUserId) is null;
        if (isNewBlock)
        {
            await _blockRepository.AddAsync(new Block
            {
                Id = Guid.NewGuid(),
                BlockerId = userId,
                BlockedId = targetUserId,
                CreatedAt = DateTime.UtcNow,
            });
        }

        // Hủy match giữa hai người (nếu có) → đồng thời chặn luôn chat (chat yêu cầu match active)
        var (a, b) = userId.CompareTo(targetUserId) < 0 ? (userId, targetUserId) : (targetUserId, userId);
        var match = await _matchRepository.GetByPairAsync(a, b);
        if (match is { IsActive: true })
            match.IsActive = false;

        await _blockRepository.SaveChangesAsync();

        // Bị block (nhẹ) — chỉ tính khi là lần block mới (fail-safe)
        if (isNewBlock)
        {
            try { await _reputationService.RecordEventAsync(targetUserId, ReputationEventType.Blocked); }
            catch { /* không để uy tín làm hỏng luồng block */ }
        }
    }

    public async Task UnblockAsync(Guid userId, Guid targetUserId)
    {
        var block = await _blockRepository.GetAsync(userId, targetUserId);
        if (block is null)
            return; // idempotent — không có gì để bỏ

        await _blockRepository.DeleteAsync(block);
        await _blockRepository.SaveChangesAsync();
        // Lưu ý: bỏ block KHÔNG tự khôi phục match cũ — hai người phải match lại từ đầu.
    }

    public async Task<List<BlockedUserDto>> GetMyBlocksAsync(Guid userId)
    {
        var blocks = await _blockRepository.GetByBlockerAsync(userId);
        if (blocks.Count == 0)
            return new List<BlockedUserDto>();

        var blockedIds = blocks.Select(b => b.BlockedId).ToList();
        var users = await _userRepository.GetWithProfileByIdsAsync(blockedIds);
        var byId = users.ToDictionary(u => u.Id);

        return blocks.Select(b =>
        {
            byId.TryGetValue(b.BlockedId, out var u);
            return new BlockedUserDto
            {
                UserId = b.BlockedId,
                DisplayName = u?.Profile?.DisplayName ?? string.Empty,
                AvatarUrl = u?.Profile?.AvatarUrl,
                BlockedAt = b.CreatedAt,
            };
        }).ToList();
    }
}
