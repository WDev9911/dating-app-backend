using SameMess.Application.DTOs.Safety;

namespace SameMess.Application.Interfaces.Services;

public interface IBlockService
{
    Task BlockAsync(Guid userId, Guid targetUserId);
    Task UnblockAsync(Guid userId, Guid targetUserId);
    Task<List<BlockedUserDto>> GetMyBlocksAsync(Guid userId);
}
